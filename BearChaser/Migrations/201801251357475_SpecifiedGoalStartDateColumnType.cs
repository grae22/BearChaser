namespace BearChaser.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SpecifiedGoalStartDateColumnType : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Goals", "StartDate", c => c.DateTime(nullable: false, precision: 7, storeType: "datetime2"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Goals", "StartDate", c => c.DateTime(nullable: false));
        }
    }
}
