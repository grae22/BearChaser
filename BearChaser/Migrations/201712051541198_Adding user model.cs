namespace BearChaser.Migrations
{
  using System.Data.Entity.Migrations;

  public partial class Addingusermodel : DbMigration
  {
    public override void Up()
    {
      CreateTable(
          "dbo.Users",
          c => new
          {
            Id = c.Int(nullable: false, identity: true),
            Username = c.String(),
            CurrentToken = c.Guid(),
          })
          .PrimaryKey(t => t.Id);

    }

    public override void Down()
    {
      DropTable("dbo.Users");
    }
  }
}
