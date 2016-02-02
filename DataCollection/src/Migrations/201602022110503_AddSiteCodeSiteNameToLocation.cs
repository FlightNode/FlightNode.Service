namespace FlightNode.DataCollection.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSiteCodeSiteNameToLocation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Locations", "SiteCode", c => c.String(nullable: false, maxLength: 100));
            AddColumn("dbo.Locations", "SiteName", c => c.String(nullable: false, maxLength: 100));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Locations", "SiteName");
            DropColumn("dbo.Locations", "SiteCode");
        }
    }
}
