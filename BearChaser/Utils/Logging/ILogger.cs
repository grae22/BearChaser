namespace BearChaser.Utils.Logging
{
  public interface ILogger
  {
    //---------------------------------------------------------------------------------------------

    void LogDebug(string message, bool includeStackTrace = false, string source = null);
    void LogInfo(string message, bool includeStackTrace = false, string source = null);
    void LogWarning(string message, bool includeStackTrace = false, string source = null);
    void LogError(string message, bool includeStackTrace = true, string source = null);

    //---------------------------------------------------------------------------------------------
  }
}
