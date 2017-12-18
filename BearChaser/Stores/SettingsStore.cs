using System.Data.Entity;
using BearChaser.Models;
using BearChaser.Utils.Logging;

namespace BearChaser.Stores
{
  internal class SettingsStore
  {
    //---------------------------------------------------------------------------------------------

    private readonly IDbSet<Setting> _settings;
    private readonly ILogger _log;

    //---------------------------------------------------------------------------------------------

    public SettingsStore(IDbSet<Setting> settings,
                         ILogger log)
    {
      _settings = settings;
      _log = log;
    }

    //---------------------------------------------------------------------------------------------

    public string GetValue(string key, string defaultValue)
    {
      Setting value = _settings.Find(new[] { key });

      if (value == null)
      {
        return defaultValue;
      }

      return value.Value;
    }

    //---------------------------------------------------------------------------------------------

    public int GetValue(string key, int defaultValue)
    {
      string valueAsString = GetValue(key, defaultValue.ToString());

      if (int.TryParse(valueAsString, out int value) == false)
      {
        _log.LogError(
          $"Setting \"{key}\" contains an invalid integer value \"{valueAsString}\". Using default value \"{defaultValue}\".");

        return defaultValue;
      }

      return value;
    }

    //---------------------------------------------------------------------------------------------
  }
}