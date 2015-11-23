using System;
using System.Data.Common;

namespace System.Data.ORM
{
    public class ColumnModel
    {
        public string PropertyName { get; set; }
        public object Value { get; set; }
        public ColumnAttribute Attribute { get; set; }     
  
        public ColumnModel(string name, object val, ColumnAttribute attr)
        {
            this.PropertyName = name;
            this.Value = val;
            this.Attribute = attr;
        }          
    }
}
