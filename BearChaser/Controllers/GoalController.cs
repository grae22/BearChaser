using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using System.Web.Http;
using AutoMapper;
using BearChaser.DataTransferObjects;
using BearChaser.Exceptions;
using BearChaser.Models;
using BearChaser.Stores;
using BearChaser.Utils.Logging;
using Newtonsoft.Json;
using WebGrease.Css.Extensions;

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

    [HttpGet]
    [Route("api/goals")]
    public async Task<IHttpActionResult> GetGoalsAsync()
    {
      User user;

      try
      {
        user = await GetUserForHeaderTokenAsync();
      }
      catch (AuthenticationException ex)
      {
        return BadRequest(ex.Message);
      }
      catch (InternalServerException)
      {
        return InternalServerError();
      }

      var goals = await _goalStore.GetGoalsAsync(user.Id);

      var goalDatas = new List<GoalData>();
      goals.ForEach(g => goalDatas.Add(Mapper.Map<GoalData>(g)));

      return Ok(JsonConvert.SerializeObject(goalDatas));
    }

    //---------------------------------------------------------------------------------------------

    [HttpPost]
    [Route("api/goals/create")]
    public async Task<IHttpActionResult> CreateGoalAsync(GoalData goalData)
    {
      User user;

      try
      {
        user = await GetUserForHeaderTokenAsync();
      }
      catch (AuthenticationException ex)
      {
        return BadRequest(ex.Message);
      }
      catch (InternalServerException)
      {
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

    private async Task<User> GetUserForHeaderTokenAsync()
    {
      string auth = Request?.Headers?.GetValues("auth").FirstOrDefault();

      if (auth == null)
      {
        throw new AuthenticationException("No token provided.");
      }

      if (Guid.TryParse(auth, out Guid userToken) == false)
      {
        throw new AuthenticationException("Invalid user token format.");
      }

      Token token = await _tokenStore.GetExistingValidTokenByGuidAsync(userToken);

      if (token == null)
      {
        throw new AuthenticationException("User token not found, it may have expired.");
      }

      User user = await _userStore.GetUserAsync(token);

      if (user == null)
      {
        _log.LogError($"User not found, but valid token exists: {token}");
        throw new InternalServerException();
      }

      return user;
    }

    //---------------------------------------------------------------------------------------------
  }
}