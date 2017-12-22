using System.Collections.Generic;

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
                         string source = null)
    {
      _logs.ForEach(l => l.LogDebug(message, includeStackTrace, source));
    }

    //---------------------------------------------------------------------------------------------

    public void LogInfo(string message,
                        bool includeStackTrace = false,
                        string source = null)
    {
      _logs.ForEach(l => l.LogInfo(message, includeStackTrace, source));
    }

    //---------------------------------------------------------------------------------------------

    public void LogWarning(string message,
                           bool includeStackTrace = false,
                           string source = null)
    {
      _logs.ForEach(l => l.LogWarning(message, includeStackTrace, source));
    }

    //---------------------------------------------------------------------------------------------

    public void LogError(string message,
                         bool includeStackTrace = true,
                         string source = null)
    {
      _logs.ForEach(l => l.LogError(message, includeStackTrace, source));
    }

    //---------------------------------------------------------------------------------------------
  }
}