namespace FlightNode.DataCollection.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SurveyCompleted : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Locations", "County", c => c.String(nullable: false, maxLength: 100));
            AddColumn("dbo.Locations", "City", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.Locations", "SiteCode", c => c.String(nullable: false, maxLength: 10));
            DropColumn("dbo.Locations", "Description");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Locations", "Description", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.Locations", "SiteCode", c => c.String(nullable: false, maxLength: 100));
            DropColumn("dbo.Locations", "City");
            DropColumn("dbo.Locations", "County");
        }
    }
}
