using System.Data.Common;
using System.Data.ORM;
using System.Data.SQLite;

namespace System.Data.SQLite.ORM
{
    public class SQLiteHelper : DbContext
    {
        public SQLiteHelper(string connStr)
            : base(connStr)
        { 
            
        }

        public override DbConnection Init()
        {
            return new SQLiteConnection(this.Database);
        }

        public override IQuery CreateQuery()
        {
            return new SQLiteQuery();
        }
         
        public override DbParameter CreateParameter(ColumnModel col)
        {
            SQLiteParameter p = new SQLiteParameter();

            p.ParameterName = "@" + col.PropertyName;
            p.Size = col.Attribute.Size;
            p.DbType = SqlMapper.TypeConvert(col.Value);  
            p.Value = col.Value;
             
            return p;
        }

        public override DataSet ExecuteDataSet(string sql, params DbParameter[] commandParameters)
        {
            //create a command and prepare it for execution
            DbCommand cmd = this.Connection.CreateCommand();

            this.SetCommand(cmd, sql, commandParameters);

            //create the DataAdapter & DataSet 
            IDataAdapter da = new SQLiteDataAdapter(cmd as SQLiteCommand);

            DataSet ds = new DataSet();

            try
            {
                //fill the DataSet using default values for DataTable names, etc.
                da.Fill(ds);

                cmd.Parameters.Clear();
            }
            catch
            {
                this.Connection.Close();

                throw;
            }

            //return the dataset
            return ds;
        }
    }
}
