using FlightNode.DataCollection.Domain.Entities;
using FlightNode.DataCollection.Domain.Interfaces.Persistence;
using System.Data.Entity;

namespace FlightNode.DataCollection.Infrastructure.Persistence
{

    public class DataCollectionContext : DbContext, ILocationPersistence, IWorkLogPersistence, IWorkTypePersistence
    {
        IDbSet<Location> IPersistenceBase<Location>.Collection
        {
            get
            {
                return this.Set<Location>();
            }
        }


        IDbSet<WorkLog> IPersistenceBase<WorkLog>.Collection
        {
            get
            {
                return this.Set<WorkLog>();
            }
        }


        IDbSet<WorkType> IPersistenceBase<WorkType>.Collection
        {
            get
            {
                return this.Set<WorkType>();
            }
        }


        public DataCollectionContext()
            : base(Properties.Settings.Default.ConnectionString)
        {
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ProxyCreationEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Location>().ToTable("Locations");
            modelBuilder.Entity<WorkType>().ToTable("WorkType");
            modelBuilder.Entity<WorkLog>().ToTable("WorkLog");
        }


        public static DataCollectionContext Create()
        {
            return new DataCollectionContext();
        }


    }
}