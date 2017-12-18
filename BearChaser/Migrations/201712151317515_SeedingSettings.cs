namespace BearChaser.Migrations
{
  using System.Data.Entity.Migrations;

  public partial class SeedingSettings : DbMigration
  {
    public override void Up()
    {
      Sql("INSERT INTO dbo.Settings(Id, Value) VALUES ('UserPasswordMinLength', '8')");
      Sql("INSERT INTO dbo.Settings(Id, Value) VALUES ('UserTokenLifetimeInMinutes', '5')");
    }

    public override void Down()
    {
    }
  }
}
