using System;
using System.Data.Common;

namespace System.Data.ORM
{
    public interface IQuery
    {
        IQuery Select(string[] cols);
        IQuery From(string name);
        IQuery Insert(string name);
        IQuery Update(string name);
        IQuery Delete();
        IQuery Where(string str);
        IQuery And(string str);
        IQuery Or(string str);
        IQuery OrderBy(string str);
        IQuery Eq(object val);
        IQuery NotEq(object val);
        IQuery Gt(object val);
        IQuery Lt(object val);
        IQuery Gte(object val);
        IQuery Lte(object val);
        IQuery Like(string str);
        IQuery Count<T>() where T : IEntity;
        IQuery Join<T>() where T : IEntity;
        IQuery LeftJoin<T>() where T : IEntity;
        IQuery As(string str);
        IQuery On(string str);
        IQuery Equal(string str);
        IQuery SortAsc();
        IQuery SortDesc();
        IQuery Set(string str);
        IQuery Symbol(string str);
        IQuery Values(string str);

        void AddParameter(DbParameter p);
        DbParameter[] GetParameters();
        string ToString();
    }
}
