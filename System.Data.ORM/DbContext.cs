using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Data.Common; 
using System.Web; 

namespace System.Data.ORM
{
    public abstract class DbContext : IDbCcontext, IDisposable
    {
        public string Database { get; private set; }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.Close();
                }
            }

            this.disposed = true;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected DbConnection _conn;
        public DbConnection Connection
        {
            get
            {
                if (this._conn == null)
                    this._conn = this.Init();

                return this._conn;
            }
        }

        private DbTransaction _trans { get; set; }

        private bool _inTrans;
        public bool InTransaction { get { return this._inTrans; } }

        public DbContext(string db_path)
        {
            this.Database = db_path; 
        }

        public abstract DbConnection Init();

        public void Close()
        {
            if (this._trans != null)
            {
                this._trans.Dispose();

                this._trans = null;
            }

            if (this._conn != null)
            {
                if (this._conn.State == ConnectionState.Open)
                    this._conn.Close();
            }               
        }

        #region Transaction

        public void StartTrans()
        {
            if (this.Connection.State != ConnectionState.Open)
                this.Connection.Open();

            this._trans = this.Connection.BeginTransaction();

            this._inTrans = true;
        }

        public void CommitTrans()
        { 
            try
            {
                this._trans.Commit();             
                    
                this._inTrans = false;
            }
            catch
            {
                throw;
            }                            
        }

        public void RollBack()
        {
            try
            {
                this._trans.Rollback();

                this._inTrans = false;
            }
            catch
            {
                throw;
            }   
        }

        #endregion    

        #region ORM 

        public virtual IQuery From<T>() where T : IEntity
        { 
            return this.Select<T>(new[] { "*" });
        }

        public virtual IQuery Select<T>(string[] cols) where T : IEntity
        {
            string table = TableMapper.GetTableName(typeof(T));

            return this.CreateQuery().Select(cols).From(table);
        }

        public virtual IList<T> Get<T>() where T : IEntity
        {
            return this.Get<T>(this.From<T>());
        }

        public virtual IList<T> Get<T>(IQuery query) where T : IEntity
        {
            using (IDataReader rdr = this.ExecuteReader(query))
            {
                return this.Get<T>(rdr);               
            }
        }

        public IList<T> Get<T>(IDataReader rdr) where T : IEntity
        {
            IList<T> list = new List<T>();

            while (rdr.Read())
            {
                T t = Activator.CreateInstance<T>();

                Type type = t.GetType();

                foreach (PropertyInfo p in type.GetProperties())
                {
                    ColumnAttribute attr = TableMapper.GetColumnAttribute(p);

                    if (attr != null)
                    {
                        object value = rdr[attr.Name] == DBNull.Value ? null : rdr[attr.Name];

                        if (attr.EnumMapping)
                        {
                            p.SetValue(t, Enum.Parse(p.PropertyType, value.ToString(), false), null);
                        }
                        else
                        {
                            p.SetValue(t, value, null);
                        }
                    }

                    if (p.Name == "IsPersisted")
                    {
                        p.SetValue(t, true, null);
                    }
                }

                FieldInfo fi = type.BaseType.GetField("_oncreate", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                fi.SetValue(t, false);
            
                list.Add(t);
            }

            rdr.Close();

            return list;
        }

        public IList<T> Get<T>(DataTable dt) where T : IEntity
        {
            IList<T> list = new List<T>();
 
            foreach (DataRow dr in dt.Rows)
            {
                T t = Activator.CreateInstance<T>();

                Type type = t.GetType();

                foreach (PropertyInfo p in type.GetProperties())
                {
                    ColumnAttribute attr = TableMapper.GetColumnAttribute(p);

                    if (attr != null)
                    {
                        object value = dr[attr.Name] == DBNull.Value ? null : dr[attr.Name];

                        if (attr.EnumMapping)
                        {
                            p.SetValue(t, Enum.Parse(p.PropertyType, value.ToString(), false), null);
                        }
                        else
                        {
                            p.SetValue(t, value, null);
                        }
                    }

                    if (p.Name == "IsPersisted")
                    {
                        p.SetValue(t, true, null);
                    }                             
                }

                FieldInfo fi = type.BaseType.GetField("_oncreate", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                fi.SetValue(t, false);

                list.Add(t);
            }
            
            return list;
        }

        public DataSet GetAll(IQuery q)
        {
            return this.ExecuteDataSet(q.ToString(), q.GetParameters());
        }
 
        public T First<T>(IQuery query) where T : IEntity
        {
            IList<T> list = this.Get<T>(query);

            if (list.Count > 0)
                return list[0];

            return default(T);
        }

        public virtual int Insert<T>(T t) where T : IEntity
        {
            if (t == null)
            {
                throw new Exception("No object to insert.");
            }
             
            List<string> fields = new List<string>();
            List<string> ps = new List<string>();

            Type type = t.GetType();
            
            string table = TableMapper.GetTableName(type);

            IQuery sql = this.CreateQuery().Insert(table);

            foreach (PropertyInfo property in type.GetProperties())
            {
                ColumnAttribute colAttr = TableMapper.GetColumnAttribute(property);

                if (colAttr != null)
                {
                    ColumnModel col = new ColumnModel(property.Name, property.GetValue(t, null), colAttr);

                    if (!col.Attribute.IsAutoInc)
                    {
                        DbParameter p = this.CreateParameter(col);

                        sql.AddParameter(p);

                        fields.Add(col.Attribute.Name);
                        ps.Add(p.ParameterName);
                    }
                }
            }

            sql.Symbol(" (" + string.Join(",", fields.ToArray()) + ")");         
            sql.Values(string.Join(",", ps.ToArray()));

            return this.ExecuteNonQuery(sql.ToString(), sql.GetParameters()); 
        }

        public IQuery Update(string table)
        {
            return this.CreateQuery().Update(table);
        }

        public virtual int Update<T>(T t) where T : IEntity
        {
            if (t == null || !t.IsPersisted)
            {
                throw new Exception("No object to update.");
            }

            TableModel table = TableMapper.Convert<T>(t);

            ColumnModel colKey = table.PrimaryKey;

            if (colKey == null)
                throw new Exception("No key column.");

            List<string> fields = new List<string>();

            IQuery query = this.Update(table.Name);

            foreach (ColumnModel col in table.Columns)
            {                 
                if (!col.Attribute.IsPrimaryKey && !col.Attribute.IsAutoInc)
                {
                    if (t.IsChanged(col.Attribute.Name))
                    {
                        DbParameter p = this.CreateParameter(col);

                        query.AddParameter(p);

                        fields.Add(col.Attribute.Name + " = " + p.ParameterName);
                    }
                }
            }

            query.Set(string.Join(",", fields.ToArray())).Where(colKey.Attribute.Name).Eq(colKey.Value);

            return this.ExecuteNonQuery(query.ToString(), query.GetParameters());           
        }

        public IQuery Delete()
        {
            return this.CreateQuery().Delete();            
        }

        public virtual int Delete<T>(T t) where T : IEntity
        {
            if (t == null || !t.IsPersisted)
                return 0;

            TableModel table = TableMapper.Convert<T>(t);

            ColumnModel colKey = table.PrimaryKey;

            if (colKey == null)
            {
                throw new Exception("No key column.");
            }

            IQuery query = this.Delete().From(table.Name).Where(colKey.Attribute.Name).Eq(colKey.Value);

            return this.ExecuteNonQuery(query.ToString(), query.GetParameters());          
        }

        public int Remove<T>() 
        {
            Type t = typeof(T);

            string table = TableMapper.GetTableName(t);

            IQuery query = this.Delete().From(table);

            return this.ExecuteNonQuery(query.ToString());
        }

        public abstract IQuery CreateQuery();
        public abstract DbParameter CreateParameter(ColumnModel col);
       
        #endregion

        #region Data Access  

        public abstract DataSet ExecuteDataSet(string sql, params DbParameter[] commandParameters);

        public virtual IDataReader ExecuteReader(IQuery query)
        {
            return this.ExecuteReader(query.ToString(), query.GetParameters());
        }

        public IDataReader ExecuteReader(string sql, params DbParameter[] commandParameters)
        {
            //create a command and prepare it for execution
            DbCommand cmd = this.Connection.CreateCommand();

            SetCommand(cmd, sql, commandParameters);

            // we use a try/catch here because if the method throws an exception we want to 
            // close the connection throw code, because no datareader will exist, hence the 
            // commandBehaviour.CloseConnection will not work
            try
            {
                IDataReader rdr = cmd.ExecuteReader();

                cmd.Parameters.Clear();

                return rdr;
            }
            catch
            {
                this.Connection.Close();

                throw;
            }
        }

        public T ExecuteScalar<T>(string sql, params DbParameter[] commandParameters)
        {
            //create a command and prepare it for execution
            DbCommand cmd = this.Connection.CreateCommand();

            SetCommand(cmd, sql, commandParameters);

            //execute the command & return the results
            T retval = (T)cmd.ExecuteScalar();

            // detach the SqlParameters from the command object, so they can be used again.
            cmd.Parameters.Clear();

            return retval;
        }

        public virtual int ExecuteNonQuery(IQuery query)
        {
            return this.ExecuteNonQuery(query.ToString(), query.GetParameters());
        }

        public int ExecuteNonQuery(string sql, params DbParameter[] commandParameters)
        {
            //create a command and prepare it for execution
            DbCommand cmd = this.Connection.CreateCommand();

            SetCommand(cmd, sql, commandParameters);

            int n = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();

            return n;
        }

        protected void SetCommand(DbCommand cmd, string cmdText, DbParameter[] commandParameters)
        {
            //Open the connection if required
            if (this.Connection.State != ConnectionState.Open)
                this.Connection.Open();

            //Set up the command
            if (this._inTrans && this._trans != null)
                cmd.Transaction = this._trans;

            cmd.Connection = this.Connection;
            cmd.CommandText = cmdText;
            cmd.CommandType = CommandType.Text;

            // Bind the parameters passed in
            if (commandParameters != null)
            {
                foreach (DbParameter parm in commandParameters)
                    cmd.Parameters.Add(parm);
            }

            return;
        }

        #endregion
    }
}
