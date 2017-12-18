namespace BearChaser.Migrations
{
  using System.Data.Entity.Migrations;

  public partial class SeedingLogSettings : DbMigration
  {
    public override void Up()
    {
      Sql("INSERT INTO dbo.Settings(Id, Value) VALUES ('LogApplicationName', 'BearChaser')");
      Sql(@"INSERT INTO dbo.Settings(Id, Value) VALUES ('LogFolderPath', 'C:\logs\')");
      Sql("INSERT INTO dbo.Settings(Id, Value) VALUES ('LogLowestPriorityCategory', 'Debug')");
    }

    public override void Down()
    {
    }
  }
}
