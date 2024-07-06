namespace AgentObserverWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class useractivityempid : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ActivityUser", "EmployeeId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ActivityUser", "EmployeeId");
        }
    }
}
