using FlightNode.Common.BaseClasses;
using System.Data.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Entity.Infrastructure;

namespace FlightNode.DataCollection.Domain.Interfaces.Persistence
{
    public interface ICrudSet<TEntity> : IDbSet<TEntity>
        where TEntity : class, IEntity
    {

    }


    public interface IPersistenceBase<TEntity>
        where TEntity : class, IEntity
    {
        int SaveChanges();
        ICrudSet<TEntity> Collection { get; }
        DbEntityEntry Entry(object entity);
    }
}
