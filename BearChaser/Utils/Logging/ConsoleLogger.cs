using System;
using System.Diagnostics;

namespace BearChaser.Utils.Logging
{
  internal class ConsoleLogger : ILogger
  {
    //---------------------------------------------------------------------------------------------

    public void LogDebug(string message,
                         bool includeStackTrace = false,
                         string source = null)
    {
      LogMessage("Debug", message, includeStackTrace, source);
    }

    //---------------------------------------------------------------------------------------------

    public void LogInfo(string message,
                        bool includeStackTrace = false,
                        string source = null)
    {
      LogMessage("Info", message, includeStackTrace, source);
    }

    //---------------------------------------------------------------------------------------------

    public void LogWarning(string message,
                           bool includeStackTrace = false,
                           string source = null)
    {
      LogMessage("Warning", message, includeStackTrace, source);
    }

    //---------------------------------------------------------------------------------------------

    public void LogError(string message,
                         bool includeStackTrace = true,
                         string source = null)
    {
      LogMessage("Error", message, includeStackTrace, source);
    }

    //---------------------------------------------------------------------------------------------

    private static void LogMessage(string category,
                                   string message,
                                   bool includeStackTrace,
                                   string source)
    {
      try
      {
        Console.WriteLine($"{DateTime.Now:u} | {category} | {source} | {message} |");

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