using FlightNode.DataCollection.Domain.Entities;
using FlightNode.DataCollection.Domain.Interfaces.Persistence;
using System.Data.Entity;

namespace FlightNode.DataCollection.Infrastructure.Persistence
{

    public class DataCollectionContext : DbContext, ILocationPersistence, IWorkLogPersistence, IWorkTypePersistence
    {
        #region Collections used by persistence interfaces that inherit from IPersistenceBase

        ICrudSet<Location> IPersistenceBase<Location>.Collection
        {
            get
            {
                return new CrudSetDecorator<Location>(this.Locations);
            }
        }


        ICrudSet<WorkLog> IPersistenceBase<WorkLog>.Collection
        {
            get
            {
                return new CrudSetDecorator<WorkLog>(this.WorkLogs);
            }
        }


        ICrudSet<WorkType> IPersistenceBase<WorkType>.Collection
        {
            get
            {
                return new CrudSetDecorator<WorkType>(this.WorkTypes);
            }
        }

        #endregion

        #region DbSets used by EF to generate the database migrations

        public DbSet<Location> Locations { get; set; }

        public DbSet<WorkLog> WorkLogs { get; set; }

        public DbSet<WorkType> WorkTypes { get; set; }

        #endregion


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