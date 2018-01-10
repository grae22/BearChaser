namespace BearChaser.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TokenValueMadeUnique : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Tokens", "Value", unique: true);
        }
        
        public override void Down()
        {
            DropIndex("dbo.Tokens", new[] { "Value" });
        }
    }
}
