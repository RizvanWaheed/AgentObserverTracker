namespace AgentObserverWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class useravlbletime : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserAvailablitiy", "AvailabilityTime", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserAvailablitiy", "AvailabilityTime");
        }
    }
}
