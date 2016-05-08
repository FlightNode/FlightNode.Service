using FlightNode.Common.BaseClasses;
using FlightNode.DataCollection.Domain.Entities;
using FlightNode.DataCollection.Domain.Interfaces.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FlightNode.DataCollection.Domain.Managers
{
    public abstract class DomainManagerBase<TEntity>
        where TEntity : class, IEntity
    {
        protected readonly IPersistenceBase<TEntity> _persistence;

        protected DomainManagerBase(IPersistenceBase<TEntity> persistence)
        {
            if (persistence == null)
            {
                throw new ArgumentNullException("persistence");
            }

            _persistence = persistence;
        }


        public virtual TEntity Create(TEntity input)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            input.Validate();

            _persistence.Collection.Add(input);
            _persistence.SaveChanges();

            // input will now have its primary key populated
            return input;
        }

        public virtual IEnumerable<TEntity> FindAll()
        {
            return _persistence.Collection
                    // When retrieving a collection - as opposed to a single item - there
                    // is no expectation that the elements will be directly updated.
                    // Therefore using AsNoTracking() for performance benefits is appropriate.
                    .AsNoTracking()
                    .ToList();
        }

        public virtual TEntity FindById(int id)
        {
            return _persistence.Collection.FirstOrDefault(x => x.Id == id);
        }

        public virtual int Update(TEntity input)
        {
            input.Validate();
            

            return _persistence.SaveChanges();            
        }

        protected virtual int ReAttachAndUpdate(TEntity input)
        {
            input.Validate();

            _persistence.Collection.Attach(input);

            SetModifiedState(_persistence, input);

            return _persistence.SaveChanges();
        }

        // This is an ugly hack because I can find no way to unit test the Entry() method, and 
        // using Entry() and setting the Modified property seems to be the only way to convinc
        // EF6 to save an object that was Attached, without having first retrieved the object 
        // from the database (which would be a completely unnecessary db call).
        public static Action<IPersistenceBase<TEntity>, TEntity> SetModifiedState = (IPersistenceBase<TEntity> persistenceLayer, TEntity input) =>
        {
            persistenceLayer.Entry(input).State = System.Data.Entity.EntityState.Modified;
        };
    }
}
