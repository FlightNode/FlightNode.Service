namespace FlightNode.DataCollection.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SurveyTypeWindSpeedRename : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SurveyCompleted", "WindSpeed", c => c.Int());
            AddColumn("dbo.SurveyPending", "WindSpeed", c => c.Int());
            DropColumn("dbo.SurveyCompleted", "WindSpeedId");
            DropColumn("dbo.SurveyPending", "WindSpeedId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SurveyPending", "WindSpeedId", c => c.Int());
            AddColumn("dbo.SurveyCompleted", "WindSpeedId", c => c.Int());
            DropColumn("dbo.SurveyPending", "WindSpeed");
            DropColumn("dbo.SurveyCompleted", "WindSpeed");
        }
    }
}
