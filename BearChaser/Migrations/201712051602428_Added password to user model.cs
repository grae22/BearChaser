namespace BearChaser.Migrations
{
  using System.Data.Entity.Migrations;

  public partial class Addedpasswordtousermodel : DbMigration
  {
    public override void Up()
    {
      AddColumn("dbo.Users", "Password", c => c.Int(nullable: false));
    }

    public override void Down()
    {
      DropColumn("dbo.Users", "Password");
    }
  }
}
