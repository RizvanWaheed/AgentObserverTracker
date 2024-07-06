namespace AgentObserverWeb.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userbreaks : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserBreaks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BreakStartTime = c.DateTime(nullable: false),
                        BreakEndTime = c.DateTime(),
                        IsBreak = c.Boolean(),
                        UserNTLogin = c.String(),
                        EmpId = c.String(),
                        CreatedDateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.UserBreaks");
        }
    }
}
