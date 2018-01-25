namespace BearChaser.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedStartDateToGoalModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Goals", "StartDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Goals", "StartDate");
        }
    }
}
