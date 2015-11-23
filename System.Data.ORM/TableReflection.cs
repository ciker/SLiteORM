using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;

namespace System.Data.ORM
{
    public class TableMapper
    {
        public static TableModel Convert<T>(T t) where T : IEntity
        {          
            Type type = t.GetType();

            TableModel table = new TableModel();

            table.Name = GetTableName(type);

            foreach (PropertyInfo property in type.GetProperties())
            {
                ColumnAttribute colAttr = GetColumnAttribute(property);

                if (colAttr != null)
                {
                    ColumnModel col = new ColumnModel(property.Name, property.GetValue(t, null), colAttr);
                                       
                    table.Columns.Add(col);  
                    
                    if (col.Attribute.IsPrimaryKey)
                    {
                        table.PrimaryKey = col;
                    }                       
                }
            }

            return table;
        }

        public static string GetTableName(Type type)      
        {            
            TableAttribute tattr = type.GetCustomAttributes(typeof(TableAttribute), false)[0] as TableAttribute;

            if (tattr == null)
                throw new Exception("No table attribute.");

            if (!string.IsNullOrEmpty(tattr.Schema))
                return tattr.Schema + "." + tattr.Name;
            else
                return tattr.Name;
        }

        public static ColumnAttribute GetColumnAttribute(PropertyInfo pi)
        {
            object[] objAttrs = pi.GetCustomAttributes(typeof(ColumnAttribute), true);

            if (objAttrs.Length > 0)
            {
                return objAttrs[0] as ColumnAttribute;
            }

            return null;
        }       
    }
}
