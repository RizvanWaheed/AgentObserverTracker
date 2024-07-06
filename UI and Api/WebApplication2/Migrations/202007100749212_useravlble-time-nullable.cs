namespace AgentObserverWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class useravlbletimenullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.UserAvailablitiy", "CheckinTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.UserAvailablitiy", "CheckoutTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.UserAvailablitiy", "CheckoutTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.UserAvailablitiy", "CheckinTime", c => c.String());
        }
    }
}
