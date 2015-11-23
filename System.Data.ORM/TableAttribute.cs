using System;
 
namespace System.Data.ORM
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Property,
                    Inherited = false, AllowMultiple = false)]
    public class TableAttribute : Attribute
    {
        public string Name;
        public string Schema;

        public TableAttribute() : this("")
        {
        }

        public TableAttribute(string name) : this(name, "")
        {
        }

        public TableAttribute(string name, string schema) : base()
        {
            this.Name = name;
            this.Schema = schema;            
        }        
    }
}
