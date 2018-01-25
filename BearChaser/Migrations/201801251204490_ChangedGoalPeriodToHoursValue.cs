namespace BearChaser.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedGoalPeriodToHoursValue : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Goals", "PeriodInHours", c => c.Int(nullable: false));
            DropColumn("dbo.Goals", "Period");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Goals", "Period", c => c.Int(nullable: false));
            DropColumn("dbo.Goals", "PeriodInHours");
        }
    }
}
