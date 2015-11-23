using System;
using System.Collections.Generic;

namespace System.Data.ORM
{
    public class TableModel
    {
        public string Name { get; set; }
        public IList<ColumnModel> Columns { get; private set; }

        public ColumnModel PrimaryKey { get; set; }

        public TableModel()
            : this("")
        {
        }

        public TableModel(string name)
        {
            this.Name = name;
            this.Columns = new List<ColumnModel>();
        }         
    }
}
