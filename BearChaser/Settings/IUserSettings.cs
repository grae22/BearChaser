namespace BearChaser.Settings
{
  public interface IUserSettings
  {
    //---------------------------------------------------------------------------------------------

    int UserPasswordMinLength { get; }
    int UserTokenLifetimeInMinutes { get; }

    //---------------------------------------------------------------------------------------------
  }
}