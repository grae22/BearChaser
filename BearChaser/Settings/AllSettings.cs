using System.Data.Entity;
using BearChaser.Models;
using BearChaser.Stores;
using BearChaser.Utils.Logging;

namespace BearChaser.Settings
{
  internal class AllSettings : IUserSettings, ILogSettings
  {
    //---------------------------------------------------------------------------------------------

    // IUserSettings
    public int UserPasswordMinLength { get; private set; }
    public int UserTokenLifetimeInMinutes { get; private set; }

    // ILogSettings
    public string ApplicationName { get; private set; }
    public string LogFolderPath { get; private set; }
    public string LowestPriorityCategoryToLog { get; private set; }

    //---------------------------------------------------------------------------------------------

    private readonly SettingsStore _settingsStore;

    //---------------------------------------------------------------------------------------------

    public AllSettings(IDbSet<Setting> settings,
                       ILogger log)
    {
      _settingsStore = new SettingsStore(settings, log);

      LoadUserSettings();
      LoadLogSettings();
    }

    //---------------------------------------------------------------------------------------------

    private void LoadUserSettings()
    {
      UserPasswordMinLength = _settingsStore.GetValue("UserPasswordMinLength", 8);
      UserTokenLifetimeInMinutes = _settingsStore.GetValue("UserTokenLifetimeInMinutes", 5);
    }

    //---------------------------------------------------------------------------------------------

    private void LoadLogSettings()
    {
      ApplicationName = _settingsStore.GetValue("LogApplicationName", "BearChaser");
      LogFolderPath = _settingsStore.GetValue("LogFolderPath", string.Empty);
      LowestPriorityCategoryToLog = _settingsStore.GetValue("LogLowestPriorityCategory", "Debug");
    }

    //---------------------------------------------------------------------------------------------
  }
}