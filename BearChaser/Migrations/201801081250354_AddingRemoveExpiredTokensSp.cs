namespace BearChaser.Migrations
{
  using System.Data.Entity.Migrations;

  public partial class AddingRemoveExpiredTokensSp : DbMigration
  {
    public override void Up()
    {
      CreateStoredProcedure(
        "sp_RemoveExpiredTokens",
        "BEGIN TRANSACTION " +
          "UPDATE dbo.Users " +
          "SET TokenId = NULL " +
          "WHERE TokenId IN (" +
            "SELECT Id " +
            "FROM dbo.Tokens " +
            "WHERE Expiry < GETUTCDATE()) " +
          "DELETE FROM dbo.Tokens " +
          "WHERE Expiry < GETUTCDATE() " +
          "COMMIT TRANSACTION");
    }

    public override void Down()
    {
      DropStoredProcedure("sp_RemoveExpiredTokens");
    }
  }
}
