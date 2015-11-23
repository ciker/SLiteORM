using System;

namespace System.Data.ORM
{
    public interface IRepository
    {
        int Add(IEntity t);  
        int Update(IEntity  t);
        int Delete(IEntity t);     
    }
}
