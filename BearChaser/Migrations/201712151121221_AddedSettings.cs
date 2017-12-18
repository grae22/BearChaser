namespace BearChaser.Migrations
{
  using System.Data.Entity.Migrations;

  public partial class AddedSettings : DbMigration
  {
    public override void Up()
    {
      CreateTable(
          "dbo.Settings",
          c => new
          {
            Id = c.String(nullable: false, maxLength: 128),
            Value = c.String(),
          })
          .PrimaryKey(t => t.Id);

    }

    public override void Down()
    {
      DropTable("dbo.Settings");
    }
  }
}
