namespace BearChaser.Migrations
{
  using System.Data.Entity.Migrations;

  public partial class AddedTokenExpiryToUserModel : DbMigration
  {
    public override void Up()
    {
      AddColumn("dbo.Users", "TokenExpiry", c => c.DateTime(nullable: false));
    }

    public override void Down()
    {
      DropColumn("dbo.Users", "TokenExpiry");
    }
  }
}
