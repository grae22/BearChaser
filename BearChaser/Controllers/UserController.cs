using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using BearChaser.Models;
using BearChaser.Settings;
using BearChaser.Stores;
using BearChaser.Utils;
using BearChaser.Utils.Logging;

namespace BearChaser.Controllers
{
  public class UserController : Controller
  {
    //---------------------------------------------------------------------------------------------

    private readonly IUserStore _userStore;
    private readonly IUserSettings _userSettings;
    private readonly IDateTimeSource _dateTimeSource;
    private readonly ILogger _log;

    //---------------------------------------------------------------------------------------------

    public UserController(IUserStore userStore,
                          IUserSettings userSettings,
                          IDateTimeSource dateTimeSource,
                          ILogger log)
    {
      _userStore = userStore;
      _userSettings = userSettings;
      _dateTimeSource = dateTimeSource;
      _log = log;
    }

    //---------------------------------------------------------------------------------------------

    [HttpPost]
    public async Task<ActionResult> Register(string username, string password)
    {
      username = username ?? string.Empty;
      password = password ?? string.Empty;

      username = username.Trim();

      if (username.Any() == false)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Username can't be blank.");
      }

      if (await _userStore.GetUserAsync(username) != null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Username already exists.");
      }

      if (password.Length < _userSettings.UserPasswordMinLength)
      {
        return new HttpStatusCodeResult(
          HttpStatusCode.BadRequest,
          $"Password must be {_userSettings.UserPasswordMinLength} or more characters long.");
      }

      int passwordHash = password.GetAsPasswordHash();

      await _userStore.AddUserAsync(username, passwordHash);

      _log.LogInfo($"User '{username}' registered.");

      return new HttpStatusCodeResult(HttpStatusCode.OK, "User registered.");
    }

    //---------------------------------------------------------------------------------------------

    [HttpPost]
    public async Task<ActionResult> Login(string username, string password)
    {
      username = username ?? string.Empty;
      password = password ?? string.Empty;

      User user = await _userStore.GetUserAsync(username);

      if (user == null)
      {
        return new HttpNotFoundResult($"Unknown username '{username}'.");
      }

      int passwordHash = password.GetAsPasswordHash();

      if (ValidatePassword(passwordHash, user) == false)
      {
        return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Invalid password.");
      }

      Guid token = await AllocateTokenAsync(user);

      _log.LogDebug($"User '{username}' logged in.");

      return new JsonResult
      {
        Data = JsonConvert.SerializeObject(token)
      };
    }

    //---------------------------------------------------------------------------------------------

    private static bool ValidatePassword(int passwordHash, User user)
    {
      return passwordHash == user.Password;
    }

    //---------------------------------------------------------------------------------------------

    private async Task<Guid> AllocateTokenAsync(User user)
    {
      DateTime now = _dateTimeSource.Now;

      if (HasValidToken(now, user))
      {
        _log.LogDebug($"Using existing valid token for user '{user.Username}'. {user.Token}");

        return user.Token.Value;
      }

      // TODO: Move out to a token issuing class.
      user.Token = new Token
      {
        Value = Guid.NewGuid(),
        Expiry = now.AddMinutes(_userSettings.UserTokenLifetimeInMinutes)
      };

      await _userStore.SaveAsync();

      _log.LogDebug($"Allocated token for user '{user.Username}'. {user.Token}");

      return user.Token.Value;
    }

    //---------------------------------------------------------------------------------------------

    private static bool HasValidToken(DateTime dateTime, User user)
    {
      if (user.Token == null)
      {
        return false;
      }

      bool isTokenStillValid = dateTime < user.Token.Expiry;

      return isTokenStillValid;
    }

    //---------------------------------------------------------------------------------------------
  }
}