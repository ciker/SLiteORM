using System;
using System.Collections.Generic;
using System.Text;

namespace System.Data.ORM
{
    public interface IDbCcontext 
    {
        IList<T> Get<T>() where T : IEntity;

        int Insert<T>(T t) where T : IEntity;

        int Update<T>(T t )where T : IEntity;

        int Delete<T>(T t) where T : IEntity;
    }
}
