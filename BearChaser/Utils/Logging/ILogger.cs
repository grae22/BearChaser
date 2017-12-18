namespace BearChaser.Utils.Logging
{
  public interface ILogger
  {
    //---------------------------------------------------------------------------------------------

    void LogDebug(string message, bool includeStackTrace = false);
    void LogInfo(string message, bool includeStackTrace = false);
    void LogWarning(string message, bool includeStackTrace = false);
    void LogError(string message, bool includeStackTrace = true);

    //---------------------------------------------------------------------------------------------
  }
}
