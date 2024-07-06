namespace AgentObserverWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initialcreate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserActivity", "PressedKey", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserActivity", "PressedKey");
        }
    }
}
