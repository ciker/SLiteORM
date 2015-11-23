using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Reflection;

namespace System.Data.ORM
{
    public abstract class SQLQuery : IQuery
    {
        public StringBuilder SqlStr;
        private IList<DbParameter> _params;

        public SQLQuery()
        {
            this.SqlStr = new StringBuilder();
            this._params = new List<DbParameter>();
        } 

        public IQuery Select(string[] cols)
        { 
            this.SqlStr.Append("SELECT ");
            this.SqlStr.Append(string.Join(",", cols));
       
            return this;
        }

        public IQuery From(string table)
        {
            this.SqlStr.Append(" FROM ");
            this.SqlStr.Append(table);

            return this;
        }

        public IQuery Insert(string table)
        {
            this.SqlStr.Append("INSERT INTO ");
            this.SqlStr.Append(table);

            return this;
        }

        public IQuery Update(string table)
        {
            this.SqlStr.Append("UPDATE ");
            this.SqlStr.Append(table);

            return this;
        }

        public IQuery Delete()
        {
            this.SqlStr.Append("DELETE");

            return this;
        }

        public IQuery Join<T>() where T : IEntity
        {
            Type type = typeof(T);

            this.SqlStr.Append(" JOIN ");
            this.SqlStr.Append(TableMapper.GetTableName(type));

            return this;
        }

        public IQuery LeftJoin<T>() where T : IEntity
        {
            Type type = typeof(T);

            this.SqlStr.Append(" LEFT JOIN ");
            this.SqlStr.Append(TableMapper.GetTableName(type));

            return this;
        }

        public IQuery Where(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                this.SqlStr.Append(" WHERE ");
                this.SqlStr.Append(str);
            }

            return this;
        }

        public IQuery And(string str)
        {
            this.SqlStr.Append(" AND ");
            this.SqlStr.Append(str);

            return this;
        }

        public IQuery Or(string str)
        {
            this.SqlStr.Append(" OR ");
            this.SqlStr.Append(str);

            return this;
        }

        public IQuery Equal(string str)
        {
            this.SqlStr.Append(" = " + str);

            return this;
        }

        public IQuery As(string str)
        {
            this.SqlStr.Append(" AS " + str);

            return this;
        }

        public IQuery On(string str)
        {
            this.SqlStr.Append(" ON " + str);

            return this;
        }

        public IQuery Symbol(string str)
        {
            this.SqlStr.Append(str);

            return this;
        }

        public IQuery OrderBy(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                this.SqlStr.Append(" ORDER BY ");
                this.SqlStr.Append(str);
            }

            return this;
        }

        public IQuery SortAsc()
        {
            this.SqlStr.Append(" ASC");

            return this;
        }

        public IQuery SortDesc()
        {
            this.SqlStr.Append(" DESC");

            return this;
        }

        public IQuery Set(string str)
        {
            this.SqlStr.Append(" SET " + str);

            return this;
        }

        public IQuery Count<T>(string str) where T : IEntity
        {
            Type type = typeof(T);

            this.SqlStr.Append("SELECT COUNT(" + str + ") FROM ");
            this.SqlStr.Append(TableMapper.GetTableName(type));

            return this;
        }

        public IQuery Count<T>() where T : IEntity
        {
            return this.Count<T>("*");
        }

        public IQuery Eq(object val)
        {
            this.SqlStr.Append(" = ");
            this.SqlStr.Append(this.AddValue(val).ParameterName);

            return this;
        }

        public IQuery NotEq(object val)
        {
            this.SqlStr.Append(" <> ");
            this.SqlStr.Append(this.AddValue(val).ParameterName);

            return this;
        }

        public IQuery Gt(object val)
        {
            this.SqlStr.Append(" > " + this.AddValue(val).ParameterName);

            return this;
        }

        public IQuery Lt(object val)
        {
            this.SqlStr.Append(" < " + this.AddValue(val).ParameterName);

            return this;
        }

        public IQuery Gte(object val)
        {
            this.SqlStr.Append(" >= " + this.AddValue(val).ParameterName);

            return this;
        }

        public IQuery Lte(object val)
        {
            this.SqlStr.Append(" <= " + this.AddValue(val).ParameterName);

            return this;
        }

        public IQuery Like(string str)
        {
            this.SqlStr.Append(" LIKE " + this.AddValue("%" + str + "%").ParameterName);

            return this;
        }

        public IQuery Values(string strs)
        {
            this.SqlStr.Append(" VALUES (" + strs + ")");

            return this;
        }

        public abstract DbParameter AddValue(object obj);
        
        public void AddParameter(DbParameter p)
        {
            this._params.Add(p);
        }

        public DbParameter[] GetParameters()
        {
            DbParameter[] retval = new DbParameter[this._params.Count];

            for (int i = 0; i < retval.Length; i++)
                retval[i] = this._params[i];

            return retval;
        }

        public int LastIndex()
        {
            return this._params.Count + 1;
        }   
        
        public override string ToString()
        {
            return this.SqlStr.ToString();
        }              
    }
}
