namespace BearChaser.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedUserToGoalModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Goals", "UserId", c => c.Int(nullable: false));
            CreateIndex("dbo.Goals", "UserId");
            AddForeignKey("dbo.Goals", "UserId", "dbo.Users", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Goals", "UserId", "dbo.Users");
            DropIndex("dbo.Goals", new[] { "UserId" });
            DropColumn("dbo.Goals", "UserId");
        }
    }
}
