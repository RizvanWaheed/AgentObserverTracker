namespace AgentObserverWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class networkavlb : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserActivity", "NetWorkActivity", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserActivity", "NetWorkActivity");
        }
    }
}
