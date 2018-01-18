using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using BearChaser.DataTransferObjects;
using BearChaser.Models;
using BearChaser.Settings;
using BearChaser.Stores;
using BearChaser.Utils;
using BearChaser.Utils.Logging;

namespace BearChaser.Controllers.Api
{
  public class UserController : ApiController
  {
    //---------------------------------------------------------------------------------------------

    private readonly IUserStore _userStore;
    private readonly ITokenStore _tokenStore;
    private readonly IUserSettings _userSettings;
    private readonly ILogger _log;

    //---------------------------------------------------------------------------------------------

    public UserController(IUserStore userStore,
                          ITokenStore tokenStore,
                          IUserSettings userSettings,
                          ILogger log)
    {
      _userStore = userStore;
      _tokenStore = tokenStore;
      _userSettings = userSettings;
      _log = log;
    }

    //---------------------------------------------------------------------------------------------

    [HttpPost]
    [Route("api/users/register")]
    public async Task<IHttpActionResult> Register(UserLoginData userLogin)
    {
      var username = userLogin.Username ?? string.Empty;
      var password = userLogin.Password ?? string.Empty;

      username = username.Trim();

      if (username.Any() == false)
      {
        return BadRequest("Username can't be blank.");
      }

      if (await _userStore.GetUserAsync(username) != null)
      {
        return BadRequest("Username already exists.");
      }

      if (password.Length < _userSettings.UserPasswordMinLength)
      {
        return BadRequest($"Password must be {_userSettings.UserPasswordMinLength} or more characters long.");
      }

      int passwordHash = password.GetAsPasswordHash();

      await _userStore.AddUserAsync(username, passwordHash);

      _log.LogInfo($"User '{username}' registered.");

      return Ok("User registered.");
    }

    //---------------------------------------------------------------------------------------------

    [HttpPost]
    [Route("api/users/login")]
    public async Task<IHttpActionResult> Login(UserLoginData userLogin)
    {
      var username = userLogin.Username ?? string.Empty;
      var password = userLogin.Password ?? string.Empty;

      User user = await _userStore.GetUserAsync(username);

      if (user == null)
      {
        return NotFound();
      }

      int passwordHash = password.GetAsPasswordHash();

      if (ValidatePassword(passwordHash, user) == false)
      {
        return Unauthorized();
      }

      Guid token = await AllocateTokenAsync(user);

      _log.LogDebug($"User logged in: {user}");

      return Ok(JsonConvert.SerializeObject(token));
    }

    //---------------------------------------------------------------------------------------------

    private static bool ValidatePassword(int passwordHash, User user)
    {
      return passwordHash == user.Password;
    }

    //---------------------------------------------------------------------------------------------

    private async Task<Guid> AllocateTokenAsync(User user)
    {
      if (_tokenStore.IsTokenValid(user.Token))
      {
        _log.LogDebug($"Using existing valid token for user '{user.Username}'. {user.Token}");

        return user.Token.Value;
      }

      Token token = await _tokenStore.GetNewTokenAsync();
      user.TokenId = token.Id;

      await _userStore.SaveAsync();

      _log.LogDebug($"Allocated token for user '{user.Username}'. {token}");

      return token.Value;
    }

    //---------------------------------------------------------------------------------------------
  }
}