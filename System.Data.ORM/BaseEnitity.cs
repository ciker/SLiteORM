using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.ORM;

namespace System.Data.ORM
{
    public abstract class BaseEnitity : IEntity
    {       
        private List<string> property;
        private bool _oncreate;

        public virtual bool IsPersisted { get; protected set; }

        public BaseEnitity()
        {
            this._oncreate = true;
        }

        public bool IsChanged(string field)
        {
            return this.property.Contains(field);
        }
               
        protected void PropertyChanged(string field)
        {
            if (!_oncreate)
            {
                if (this.property == null)
                    this.property = new List<string>();

                if (!this.property.Contains(field))
                {
                    this.property.Add(field);
                }
            }
        }
    }
}
