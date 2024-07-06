namespace AgentObserverWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class useravailablitytable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserAvailablitiy",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CheckinTime = c.String(),
                        CheckoutTime = c.DateTime(nullable: false),
                        UserNTLogin = c.String(),
                        IsCheckedIn = c.Boolean(nullable: false),
                        IsCheckedOut = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.UserAvailablitiy");
        }
    }
}
