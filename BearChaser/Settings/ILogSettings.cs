namespace BearChaser.Settings
{
  internal interface ILogSettings
  {
    //---------------------------------------------------------------------------------------------

    string ApplicationName { get; }
    string LogFolderPath { get; }
    string LowestPriorityCategoryToLog { get; }

    //---------------------------------------------------------------------------------------------
  }
}
