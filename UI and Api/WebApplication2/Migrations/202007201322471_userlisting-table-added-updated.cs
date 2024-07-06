namespace AgentObserverWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userlistingtableaddedupdated : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ActivityUser",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ActivityUser");
        }
    }
}
