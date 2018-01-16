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

    private void LogMessage(string category,
                            string message,
                            bool includeStackTrace,
                            string sourceFilePath = null,
                            string sourceMember = null,
                            int sourceLine = -1)
    {
      try
      {
        if (_writer == null)
        {
          return;
        }

        string source = BuildSourceString(sourceFilePath, sourceMember, sourceLine);

        lock (_writeToFileLock)
        {
          _writer.WriteLine($"{DateTime.Now:u} | {category} | {message} | {source} |");

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