using System;
using System.Diagnostics;

namespace BearChaser.Utils.Logging
{
  internal class ConsoleLogger : ILogger
  {
    //---------------------------------------------------------------------------------------------

    public void LogDebug(string message, bool includeStackTrace = false)
    {
      LogMessage("Debug", message, includeStackTrace);
    }

    //---------------------------------------------------------------------------------------------

    public void LogInfo(string message, bool includeStackTrace = false)
    {
      LogMessage("Info", message, includeStackTrace);
    }

    //---------------------------------------------------------------------------------------------

    public void LogWarning(string message, bool includeStackTrace = false)
    {
      LogMessage("Warning", message, includeStackTrace);
    }

    //---------------------------------------------------------------------------------------------

    public void LogError(string message, bool includeStackTrace = true)
    {
      LogMessage("Error", message, includeStackTrace);
    }

    //---------------------------------------------------------------------------------------------

    private static void LogMessage(string category, string message, bool includeStackTrace)
    {
      try
      {
        Console.WriteLine($"{DateTime.Now:u} | {category} | {message} |");

        if (includeStackTrace)
        {
          Console.WriteLine(new StackTrace().ToString());
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine($"ERROR: Error while logging to console - {ex.Message}");
      }
    }

    //---------------------------------------------------------------------------------------------
  }
}