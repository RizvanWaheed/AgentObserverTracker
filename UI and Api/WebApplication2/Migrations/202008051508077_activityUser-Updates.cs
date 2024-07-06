namespace AgentObserverWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class activityUserUpdates : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ActivityUser", "DepartmentName", c => c.String());
            AddColumn("dbo.ActivityUser", "TeamName", c => c.String());
            AddColumn("dbo.ActivityUser", "ManagerOrSupervisorName", c => c.String());
            AddColumn("dbo.ActivityUser", "Location", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ActivityUser", "Location");
            DropColumn("dbo.ActivityUser", "ManagerOrSupervisorName");
            DropColumn("dbo.ActivityUser", "TeamName");
            DropColumn("dbo.ActivityUser", "DepartmentName");
        }
    }
}
