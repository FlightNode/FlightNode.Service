using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FlightNode.DataCollection.Domain.UnitTests.Domain.Managers
{
    public class FakeDbSet<TEntity> : IDbSet<TEntity>
        where TEntity : class
    {
        public List<TEntity> List = new List<TEntity>();

        public Type ElementType
        {
            get
            {
                return typeof(TEntity);
            }
        }

        public Expression Expression
        {
            get
            {
                return List.AsQueryable().Expression;
            }
        }

        public ObservableCollection<TEntity> Local
        {
            get
            {
                return new ObservableCollection<TEntity>(List);
            }
        }

        public IQueryProvider Provider
        {
            get
            {
                return List.AsQueryable().Provider;
            }
        }

        public TEntity Add(TEntity entity)
        {
            List.Add(entity);
            return entity;
        }

        public TEntity Attach(TEntity entity)
        {
            return Add(entity);
        }

        public TEntity Create()
        {
            return Activator.CreateInstance<TEntity>();
        }

        public TDerivedEntity Create<TDerivedEntity>() where TDerivedEntity : class, TEntity
        {
            throw new NotImplementedException();
        }

        public TEntity Find(params object[] keyValues)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<TEntity> GetEnumerator()
        {
            return List.GetEnumerator();
        }

        public TEntity Remove(TEntity entity)
        {
            List.Remove(entity);
            return entity;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return List.GetEnumerator();
        }
    }
}
