using System;
using System.Diagnostics;
using System.IO;
using BearChaser.Settings;

namespace BearChaser.Utils.Logging
{
  internal class FileLogger : ILogger
  {
    //---------------------------------------------------------------------------------------------

    private readonly string _applicationName;
    private readonly string _logFolderPath;
    private readonly object _writeToFileLock = new object();
    private StreamWriter _writer;

    //---------------------------------------------------------------------------------------------

    public FileLogger(ILogSettings settings)
    {
      _applicationName = settings.ApplicationName ?? "UnknownApplication";
      _logFolderPath = settings.LogFolderPath ?? string.Empty;

      CreateFileWriter();
      LogInfo("Logger initialised.");
    }

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
    
    private void CreateFileWriter()
    {
      try
      {
        _writer = new StreamWriter(
          $"{_logFolderPath}{Path.DirectorySeparatorChar}{_applicationName}_{DateTime.Now:yyyy-MM-dd}.log",
          true)
        {
          AutoFlush = true
        };
      }
      catch (Exception ex)
      {
        Console.WriteLine($"ERROR: Error while creating log file writer - {ex.Message}");
      }
    }

    //---------------------------------------------------------------------------------------------

    private void LogMessage(string category, string message, bool includeStackTrace)
    {
      try
      {
        if (_writer == null)
        {
          return;
        }

        lock (_writeToFileLock)
        {
          _writer.WriteLine($"{DateTime.Now:u} | {category} | {message} |");

          if (includeStackTrace)
          {
            _writer.WriteLine(new StackTrace().ToString());
          }
        }
      }
      catch (Exception)
      {
        // We ignore any exceptions raised while writing to the log file.
      }
    }

    //---------------------------------------------------------------------------------------------
  }
}