using System;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using System.Web.Http;
using BearChaser.Exceptions;
using BearChaser.Models;
using BearChaser.Stores;
using BearChaser.Utils.Logging;

namespace BearChaser.Controllers.Utils
{
  internal static class ControllerUtils
  {
    //---------------------------------------------------------------------------------------------
    
    public static async Task<User> GetUserForRequestHeaderTokenAsync(ApiController controller,
                                                              ITokenStore tokenStore,
                                                              IUserStore userStore,
                                                              ILogger log)
    {
      if (controller.Request?.Headers?.Contains("auth") == false)
      {
        log.LogDebug("No token provided.");
        throw new AuthenticationException("No token provided.");
      }

      string auth = controller.Request?.Headers?.GetValues("auth").FirstOrDefault();
      
      if (Guid.TryParse(auth, out Guid userToken) == false)
      {
        log.LogDebug($"Invalid token format \"{auth}\".");
        throw new AuthenticationException("Invalid user token format.");
      }

      Token token = await tokenStore.GetExistingValidTokenByGuidAsync(userToken);

      if (token == null)
      {
        log.LogDebug($"Token not found \"{auth}\".");
        throw new AuthenticationException("User token not found, it may have expired.");
      }

      User user = await userStore.GetUserAsync(token);

      if (user == null)
      {
        log.LogError($"User not found, but valid token exists: {token}");
        throw new InternalServerException();
      }

      log.LogDebug($"Found user \"{user.Username}\" for token {token}.");

      return user;
    }

    //---------------------------------------------------------------------------------------------
  }
}