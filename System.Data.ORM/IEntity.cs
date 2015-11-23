using System;

namespace System.Data.ORM
{
    public interface IEntity
    {
        bool IsPersisted { get; }
        bool IsChanged(string field);        
    }
}
