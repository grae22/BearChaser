using System;
using System.Diagnostics;
using System.IO;

namespace BearChaser.Utils.Logging
{
  internal class ConsoleLogger : ILogger
  {
    //---------------------------------------------------------------------------------------------

    public void LogDebug(string message,
                         bool includeStackTrace = false,
                         string sourceFilePath = null,
                         string sourceMember = null,
                         int sourceLine = -1)
    {
      LogMessage("Debug", message, includeStackTrace, sourceFilePath, sourceMember, sourceLine);
    }

    //---------------------------------------------------------------------------------------------

    public void LogInfo(string message,
                        bool includeStackTrace = false,
                        string sourceFilePath = null,
                        string sourceMember = null,
                        int sourceLine = -1)
    {
      LogMessage("Info", message, includeStackTrace, sourceFilePath, sourceMember, sourceLine);
    }

    //---------------------------------------------------------------------------------------------

    public void LogWarning(string message,
                           bool includeStackTrace = false,
                           string sourceFilePath = null,
                           string sourceMember = null,
                           int sourceLine = -1)
    {
      LogMessage("Warning", message, includeStackTrace, sourceFilePath, sourceMember, sourceLine);
    }

    //---------------------------------------------------------------------------------------------

    public void LogError(string message,
                         bool includeStackTrace = true,
                         string sourceFilePath = null,
                         string sourceMember = null,
                         int sourceLine = -1)
    {
      LogMessage("Error", message, includeStackTrace, sourceFilePath, sourceMember, sourceLine);
    }

    //---------------------------------------------------------------------------------------------

    private static void LogMessage(string category,
                                   string message,
                                   bool includeStackTrace,
                                   string sourceFilePath = null,
                                   string sourceMember = null,
                                   int sourceLine = -1)
    {
      try
      {
        string source = BuildSourceString(sourceFilePath, sourceMember, sourceLine);

        Console.WriteLine($"{DateTime.Now:u} | {category} | {message} | {source} |");

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

    private static string BuildSourceString(string filePath,
                                            string memberName,
                                            int line)
    {
      filePath = filePath ?? string.Empty;
      memberName = memberName ?? string.Empty;

      return $"{Path.GetFileName(filePath)}({line}):{memberName}()";
    }

    //---------------------------------------------------------------------------------------------
  }
}