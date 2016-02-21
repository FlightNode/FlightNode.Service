using Dapper;
using FlightNode.DataCollection.Domain.Entities;
using FlightNode.DataCollection.Domain.Interfaces.Persistence;
using System.Collections.Generic;
using System.Data.Entity;

namespace FlightNode.DataCollection.Infrastructure.Persistence
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class DataCollectionContext : DbContext, ILocationPersistence, IWorkLogPersistence, IWorkTypePersistence, IBirdSpeciesPersistence, ISurveyTypePersistence, ISurveyPersistence
    {
        #region Collections used by persistence interfaces that inherit from IPersistenceBase

        ICrudSet<SurveyType> IPersistenceBase<SurveyType>.Collection
        {
            get
            {
                return new CrudSetDecorator<SurveyType>(this.SurveyTypes);
            }
        }

        ICrudSet<BirdSpecies> IPersistenceBase<BirdSpecies>.Collection
        {
            get
            {
                return new CrudSetDecorator<BirdSpecies>(this.BirdSpecies);
            }
        }

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

        #region ISurveyPersistence

        ICrudSet<Location> ISurveyPersistence.Locations
        {
            get
            {
                return new CrudSetDecorator<Location>(this.Locations);
            }
        }

        ICrudSet<SurveyPending> ISurveyPersistence.SurveysPending
        {
            get
            {
                return new CrudSetDecorator<SurveyPending>(this.SurveysPending);
            }
        }

        ICrudSet<SurveyCompleted> ISurveyPersistence.SurveysCompleted
        {
            get
            {
                return new CrudSetDecorator<SurveyCompleted>(this.SurveysCompleted);
            }
        }

        ICrudSet<Disturbance> ISurveyPersistence.Disturbances
        {
            get
            {
                return new CrudSetDecorator<Disturbance>(this.Disturbances);
            }
        }
        ICrudSet<Observation> ISurveyPersistence.Observations
        {
            get
            {
                return new CrudSetDecorator<Observation>(this.Observations);
            }
        }
        #endregion


        #region Specialized Queries for IWorkLogPersistence

        public IEnumerable<WorkLogReportRecord> GetWorkLogReportRecords()
        {
            using (var conn = this.Database.Connection)
            {
                if (conn.State != System.Data.ConnectionState.Open)
                {
                    conn.Open();
                }

                return conn.Query<WorkLogReportRecord>("SELECT * FROM dbo.WorkLogReport");
            }

        }

        public IEnumerable<WorkLogReportRecord> GetWorkLogReportRecords(int userId)
        {
            using (var conn = this.Database.Connection)
            {
                if (conn.State != System.Data.ConnectionState.Open)
                {
                    conn.Open();
                }

                return conn.Query<WorkLogReportRecord>("SELECT * FROM dbo.WorkLogReport WHERE UserId = @UserId", new { UserId = userId });
            }

        }

        #endregion


        #region EF DbSets 

        public DbSet<Location> Locations { get; set; }

        public DbSet<WorkLog> WorkLogs { get; set; }

        public DbSet<WorkType> WorkTypes { get; set; }

        public DbSet<BirdSpecies> BirdSpecies { get; set; }

        public DbSet<SurveyType> SurveyTypes { get; set; }

        public DbSet<SurveyCompleted> SurveysCompleted { get; set; }

        public DbSet<SurveyPending> SurveysPending { get; set; }

        public DbSet<Disturbance> Disturbances { get; set; }

        public DbSet<DisturbanceType> DisturbanceType { get; set; }

        public DbSet<Observation> Observations { get; set; }

        public DbSet<Observer> Observers { get; set; }

        public DbSet<Tide> Tides { get; set; }

        public DbSet<Weather> Weather { get; set; }

        public DbSet<HabitatType> HabitatTypes { get; set; }

        public DbSet<AccessPoint> AccessPoints { get; set; }

        public DbSet<SiteAssessment> SiteAssessments { get; set; }

        public DbSet<VantagePoint> VantagePoints { get; set; }

        public DbSet<FeedingSuccessRate> FeedingSuccessRates { get; set; }

        public DbSet<SurveyActivity> SurveyActivities { get; set; }
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

            // Entity framework truncates decimal data types to two places unless you tell it otherwise. How dumb.
            // Geographic coordinates need 6 digits to the right of the decimal point, and at most 3 to the left.
            modelBuilder.Entity<Location>().Property(x => x.Longitude).HasPrecision(9, 6);
            modelBuilder.Entity<Location>().Property(x => x.Latitude).HasPrecision(9, 6);


            modelBuilder.Entity<SurveyType>().ToTable("SurveyType");
            modelBuilder.Entity<BirdSpecies>()
                .ToTable("BirdSpecies")
                .HasMany(x => x.SurveyTypes)
                .WithMany(x => x.BirdSpecies)
                .Map(m =>
               {
                   m.MapLeftKey("BirdSpeciesId");
                   m.MapRightKey("SurveyTypeId");
                   m.ToTable("SurveyType_BirdSpecies");
               });

            modelBuilder.Entity<DisturbanceType>()
                .ToTable("DisturbanceTypes");

            modelBuilder.Entity<Disturbance>()
                .ToTable("Disturbances")
                .HasRequired(x => x.DisturbanceType)
                .WithMany(x => x.Disturbances);

            modelBuilder.Entity<SurveyCompleted>().ToTable("SurveyCompleted");

            // Observer has a foreign key to User, but User is in in the 
            // Identity project. Identity should not have a reference to 
            // the DataCollection project; User cannot have a navigation
            // property back to Observer; and therefore we must manually
            // add the foreign key relationship in the migration script

            modelBuilder.Entity<Weather>().ToTable("Weather");
            modelBuilder.Entity<Tide>().ToTable("Tides");

            modelBuilder.Entity<SurveyPending>().ToTable("SurveyPending");

            modelBuilder.Entity<HabitatType>().ToTable("HabitatTypes");
            modelBuilder.Entity<SurveyActivity>().ToTable("SurveyActivities");
            modelBuilder.Entity<SiteAssessment>().ToTable("SiteAssessments");
            modelBuilder.Entity<VantagePoint>().ToTable("VantagePoints");
            modelBuilder.Entity<AccessPoint>().ToTable("AccessPoints");
            modelBuilder.Entity<FeedingSuccessRate>().ToTable("FeedingSuccessRates");
        }


        public static DataCollectionContext Create()
        {
            return new DataCollectionContext();
        }
    }
}