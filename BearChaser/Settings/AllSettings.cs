using System.Data.Entity;
using BearChaser.Models;
using BearChaser.Stores;
using BearChaser.Utils.Logging;

namespace BearChaser.Settings
{
  internal class AllSettings : IUserSettings, ILogSettings, ITokenSettings
  {
    //---------------------------------------------------------------------------------------------

    // IUserSettings
    public int UserPasswordMinLength { get; private set; } = 8;

    // ILogSettings
    public string ApplicationName { get; private set; } = "BearChaser";
    public string LogFolderPath { get; private set; } = string.Empty;
    public string LowestPriorityCategoryToLog { get; private set; } = "Debug";

    // ITokenSettings
    public int TokenLifetimeInMinutes { get; private set; } = 5;

    //---------------------------------------------------------------------------------------------

    private readonly SettingsStore _settingsStore;

    //---------------------------------------------------------------------------------------------

    public AllSettings(IDbSet<Setting> settings,
                       ILogger log)
    {
      _settingsStore = new SettingsStore(settings, log);

      LoadUserSettings();
      LoadLogSettings();
      LoadTokenSettings();
    }

    //---------------------------------------------------------------------------------------------

    private void LoadUserSettings()
    {
      UserPasswordMinLength = _settingsStore.GetValue("UserPasswordMinLength", UserPasswordMinLength);
    }

    //---------------------------------------------------------------------------------------------

    private void LoadLogSettings()
    {
      ApplicationName = _settingsStore.GetValue("LogApplicationName", ApplicationName);
      LogFolderPath = _settingsStore.GetValue("LogFolderPath", LogFolderPath);
      LowestPriorityCategoryToLog = _settingsStore.GetValue("LogLowestPriorityCategory", LowestPriorityCategoryToLog);
    }

    //---------------------------------------------------------------------------------------------

    private void LoadTokenSettings()
    {
      TokenLifetimeInMinutes = _settingsStore.GetValue("TokenLifetimeInMinutes", TokenLifetimeInMinutes);
    }

    //---------------------------------------------------------------------------------------------
  }
}