using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace BearChaser.Utils.Logging
{
  internal class Log : ILogger
  {
    //---------------------------------------------------------------------------------------------

    private readonly List<ILogger> _logs = new List<ILogger>();
    
    //---------------------------------------------------------------------------------------------
    
    public void RegisterLogger(ILogger log)
    {
      _logs.Add(log);
    }

    //---------------------------------------------------------------------------------------------

    public void LogDebug(string message,
                         bool includeStackTrace = false,
                         [CallerFilePath] string sourceFilePath = null,
                         [CallerMemberName] string sourceMember = null,
                         [CallerLineNumber] int sourceLine = -1)
    {
      _logs.ForEach(l => l.LogDebug(
        message,
        includeStackTrace,
        sourceFilePath,
        sourceMember,
        sourceLine));
    }

    //---------------------------------------------------------------------------------------------

    public void LogInfo(string message,
                        bool includeStackTrace = false,
                        [CallerFilePath] string sourceFilePath = null,
                        [CallerMemberName] string sourceMember = null,
                        [CallerLineNumber] int sourceLine = -1)
    {
      _logs.ForEach(l => l.LogInfo(
        message,
        includeStackTrace,
        sourceFilePath,
        sourceMember,
        sourceLine));
    }

    //---------------------------------------------------------------------------------------------

    public void LogWarning(string message,
                           bool includeStackTrace = false,
                           [CallerFilePath] string sourceFilePath = null,
                           [CallerMemberName] string sourceMember = null,
                           [CallerLineNumber] int sourceLine = -1)
    {
      _logs.ForEach(l => l.LogWarning(
        message,
        includeStackTrace,
        sourceFilePath,
        sourceMember,
        sourceLine));
    }

    //---------------------------------------------------------------------------------------------

    public void LogError(string message,
                         bool includeStackTrace = true,
                         [CallerFilePath] string sourceFilePath = null,
                         [CallerMemberName] string sourceMember = null,
                         [CallerLineNumber] int sourceLine = -1)
    {
      _logs.ForEach(l => l.LogError(
        message,
        includeStackTrace,
        sourceFilePath,
        sourceMember,
        sourceLine));
    }

    //---------------------------------------------------------------------------------------------
  }
}