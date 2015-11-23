using System;

namespace System.Data.ORM
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field,
                    Inherited = false, AllowMultiple = false)]
    public class ColumnAttribute : Attribute
    {
        public string Name { get; private set; }
        public int Size { get; private set; }
        public bool IsPrimaryKey { get; set;}
        public bool IsForeignkey { get; set; }
        public bool IsAutoInc { get; set; }
        public bool EnumMapping { get; private set; }

        public ColumnAttribute(string name)
            : this(name, 0)
        {
        }

        public ColumnAttribute(string name, int size)
            : this(name, size, false)
        {          
        }

        public ColumnAttribute(string name,int size, bool isEnum)
        {
            this.Name = name;           
            this.Size = size;
            this.EnumMapping = isEnum;
        }
    }
}
