using System.Runtime.CompilerServices;

namespace BearChaser.Utils.Logging
{
  public interface ILogger
  {
    //---------------------------------------------------------------------------------------------

    void LogDebug(string message, bool includeStackTrace = false, [CallerFilePath] string sourceFilePath = null, [CallerMemberName] string sourceMember = null, [CallerLineNumber] int sourceLine = -1);
    void LogInfo(string message, bool includeStackTrace = false, [CallerFilePath] string sourceFilePath = null, [CallerMemberName] string sourceMember = null, [CallerLineNumber] int sourceLine = -1);
    void LogWarning(string message, bool includeStackTrace = false, [CallerFilePath] string sourceFilePath = null, [CallerMemberName] string sourceMember = null, [CallerLineNumber] int sourceLine = -1);
    void LogError(string message, bool includeStackTrace = true, [CallerFilePath] string sourceFilePath = null, [CallerMemberName] string sourceMember = null, [CallerLineNumber] int sourceLine = -1);

    //---------------------------------------------------------------------------------------------
  }
}
