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

        public IEnumerable<TEntity> FindAll()
        {
            return _persistence.Collection.ToList();
        }

        public TEntity FindById(int id)
        {
            return _persistence.Collection.FirstOrDefault(x => x.Id == id);
        }

        public void Update(TEntity input)
        {
            input.Validate();

            _persistence.Collection.Attach(input);
            _persistence.SaveChanges();
        }
    }
}
