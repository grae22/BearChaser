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

    public void LogDebug(string message, bool includeStackTrace = false)
    {
      _logs.ForEach(l => l.LogDebug(message, includeStackTrace));
    }

    //---------------------------------------------------------------------------------------------

    public void LogInfo(string message, bool includeStackTrace = false)
    {
      _logs.ForEach(l => l.LogInfo(message, includeStackTrace));
    }

    //---------------------------------------------------------------------------------------------

    public void LogWarning(string message, bool includeStackTrace = false)
    {
      _logs.ForEach(l => l.LogWarning(message, includeStackTrace));
    }

    //---------------------------------------------------------------------------------------------

    public void LogError(string message, bool includeStackTrace = true)
    {
      _logs.ForEach(l => l.LogError(message, includeStackTrace));
    }

    //---------------------------------------------------------------------------------------------
  }
}