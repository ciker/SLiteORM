using System;
using System.Collections.Generic;
using System.Text;
using System.Data.ORM;

namespace System.Data.ORM
{
    public abstract class Repository : IRepository 
    {
        public DbContext Db { get; set; }
 
        public Repository()
        {             
        }

        public virtual void Dispose()
        {
            this.Db.Close();
        }
         
        public void Begin()
        {
            this.Db.StartTrans();   
        }

        //commit sql transaction
        public void SaveChanges()
        {
            if (this.Db.InTransaction)
                this.Db.CommitTrans();
        }

        public void Cancel()
        {
            this.Db.RollBack();
        }

        public virtual List<T> ToList<T>(IList<T> list)
        {
            return new List<T>(list);
        }

        #region DB CURD

        public virtual int Add(IEntity t )
        {
            return this.Db.Insert(t);
        }

        public virtual int Update(IEntity t)
        {
            return this.Db.Update(t);
        }

        public virtual int Save(IEntity t)
        {
            if (t.IsPersisted)
                return this.Db.Update(t);
            else
                return this.Db.Insert(t);
        }

        public virtual int Delete(IEntity t)
        {
            return this.Db.Delete(t);
        }
 
        #endregion
    }
}
