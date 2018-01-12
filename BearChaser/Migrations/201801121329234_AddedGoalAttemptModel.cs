namespace BearChaser.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedGoalAttemptModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GoalAttempts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GoalId = c.Int(nullable: false),
                        Timestamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Goals", t => t.GoalId, cascadeDelete: true)
                .Index(t => t.GoalId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GoalAttempts", "GoalId", "dbo.Goals");
            DropIndex("dbo.GoalAttempts", new[] { "GoalId" });
            DropTable("dbo.GoalAttempts");
        }
    }
}
