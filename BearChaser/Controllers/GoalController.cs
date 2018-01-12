using System;
using System.Threading.Tasks;
using System.Web.Http;
using BearChaser.DataTransferObjects;
using BearChaser.Models;
using BearChaser.Stores;
using BearChaser.Utils.Logging;

namespace BearChaser.Controllers
{
  public class GoalController : ApiController
  {
    //---------------------------------------------------------------------------------------------

    private readonly IGoalStore _goalStore;
    private readonly IUserStore _userStore;
    private readonly ITokenStore _tokenStore;
    private readonly ILogger _log;

    //---------------------------------------------------------------------------------------------

    public GoalController(IGoalStore goalStore,
                          IUserStore userStore,
                          ITokenStore tokenStore,
                          ILogger log)
    {
      if (goalStore == null)
      {
        throw new ArgumentException("GoalStore cannot be null.");
      }

      if (userStore == null)
      {
        throw new ArgumentException("UserStore cannot be null.");
      }

      if (tokenStore == null)
      {
        throw new ArgumentException("TokenStore cannot be null.");
      }

      if (log == null)
      {
        throw new ArgumentException("Logger cannot be null.");
      }

      _goalStore = goalStore;
      _userStore = userStore;
      _tokenStore = tokenStore;
      _log = log;
    }

    //---------------------------------------------------------------------------------------------

    [HttpPost]
    [Route("api/goal/create")]
    public async Task<IHttpActionResult> CreateGoalAsync(GoalData goalData)
    {
      if (Guid.TryParse(goalData.UserToken, out Guid userToken) == false)
      {
        return BadRequest("Invalid user token format.");
      }

      var token = await _tokenStore.GetExistingValidTokenByGuidAsync(userToken);

      if (token == null)
      {
        return BadRequest("User token not found, it may have expired.");
      }

      var user = await _userStore.GetUserAsync(token);

      if (user == null)
      {
        _log.LogError($"User not found, but valid token exists: {token}");
        return InternalServerError();
      }

      Goal goal =
        await _goalStore.CreateGoalAsync(
          user.Id,
          goalData.Name,
          (Goal.TimePeriod)goalData.Period,
          goalData.FrequencyWithinPeriod);

      _log.LogDebug($"Goal created: {goal}");

      return Ok();
    }

    //---------------------------------------------------------------------------------------------
  }
}