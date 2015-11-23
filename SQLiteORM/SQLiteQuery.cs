using System.Data.Common;
using System.Data.ORM;
using System.Data.SQLite;

namespace System.Data.SQLite.ORM
{
    public class SQLiteQuery : SQLQuery
    {
        public SQLQuery Skip(int start)
        {
            this.SqlStr.Append(" LIMIT " + start.ToString());

            return this;
        }

        public SQLQuery Take(int count)
        {
            this.SqlStr.Append(" , " + count.ToString());

            return this;
        }

        public override DbParameter AddValue(object obj)
        {
            string name = "@p" + this.LastIndex();

            SQLiteParameter p = new SQLiteParameter(name, obj);

            p.DbType = SqlMapper.TypeConvert(obj);

            this.AddParameter(p);

            return p;
        }          
    }
}
