namespace AgentObserverWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userlistingtableadded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserAvailablitiy", "BreakStartTime", c => c.DateTime());
            AddColumn("dbo.UserAvailablitiy", "BreakEndTime", c => c.DateTime());
            AddColumn("dbo.UserAvailablitiy", "IsBreak", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserAvailablitiy", "IsBreak");
            DropColumn("dbo.UserAvailablitiy", "BreakEndTime");
            DropColumn("dbo.UserAvailablitiy", "BreakStartTime");
        }
    }
}
