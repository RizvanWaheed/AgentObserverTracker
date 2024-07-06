namespace AgentObserverWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class activtyType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserActivity", "ActivityType", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserActivity", "ActivityType");
        }
    }
}
