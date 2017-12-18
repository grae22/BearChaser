namespace BearChaser.Migrations
{
  using System.Data.Entity.Migrations;

  public partial class AddedTokenModel : DbMigration
  {
    public override void Up()
    {
      CreateTable(
          "dbo.Tokens",
          c => new
          {
            Id = c.Int(nullable: false, identity: true),
            Value = c.Guid(nullable: false),
            Expiry = c.DateTime(nullable: false),
          })
        .PrimaryKey(t => t.Id);

      AddColumn("dbo.Users", "TokenId", c => c.Int());
      CreateIndex("dbo.Users", "TokenId");
      AddForeignKey("dbo.Users", "TokenId", "dbo.Tokens", "Id");
      DropColumn("dbo.Users", "CurrentToken");
      DropColumn("dbo.Users", "TokenExpiry");
    }

    public override void Down()
    {
      AddColumn("dbo.Users", "TokenExpiry", c => c.DateTime(nullable: false));
      AddColumn("dbo.Users", "CurrentToken", c => c.Guid());
      DropForeignKey("dbo.Users", "TokenId", "dbo.Tokens");
      DropIndex("dbo.Users", new[] {"TokenId"});
      DropColumn("dbo.Users", "TokenId");
      DropTable("dbo.Tokens");
    }
  }
}
