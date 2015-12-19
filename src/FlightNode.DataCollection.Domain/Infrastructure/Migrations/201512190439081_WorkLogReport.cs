using System;
using System.Data.Entity.Migrations;
using FlightNode.DataCollection.Infrastructure.Customization;

namespace FlightNode.DataCollection.Domain.Migrations
{
	
	public partial class WorkLogReport : DbMigration
	{
		public override void Up()
		{
			this.CreateView("dbo.WorkLogReport", @"SELECT wl.Id, 
	wl.WorkDate, 
	wl.WorkHours, 
	wl.TravelTimeHours, 
	wt.Description as WorkType, 
	l.Description as LocationName, 
	l.Longitude, 
	l.Latitude, 
	u.GivenName + ' ' + u.FamilyName as DisplayName
FROM dbo.worklog wl
INNER JOIN dbo.WorkType wt ON wl.WorkTypeId = wt.Id
INNER JOIN dbo.Locations l ON wl.LocationId = l.Id
INNER JOIN dbo.Users u ON wl.UserId = u.Id
");
		}

		public override void Down()
		{
			this.DropView("dbo.WorkLogReport");
		}
	}
}
