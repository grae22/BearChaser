using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using System.Web.Http;
using AutoMapper;
using Newtonsoft.Json;
using WebGrease.Css.Extensions;
using BearChaser.Controllers.Utils;
using BearChaser.DataTransferObjects;
using BearChaser.Exceptions;
using BearChaser.Models;
using BearChaser.Stores;
using BearChaser.Utils;
using BearChaser.Utils.Logging;

namespace BearChaser.Controllers.Api
{
  public class GoalController : ApiController
  {
    //---------------------------------------------------------------------------------------------

    private readonly IGoalStore _goalStore;
    private readonly IGoalAttemptStore _goalAttemptStore;
    private readonly IUserStore _userStore;
    private readonly ITokenStore _tokenStore;
    private readonly IDateTimeSource _dateTime;
    private readonly ILogger _log;

    //---------------------------------------------------------------------------------------------

    public GoalController(IGoalStore goalStore,
                          IGoalAttemptStore goalAttemptStore,
                          IUserStore userStore,
                          ITokenStore tokenStore,
                          IDateTimeSource dateTime,
                          ILogger log)
    {
      if (goalStore == null)
      {
        throw new ArgumentException("GoalStore cannot be null.");
      }

      if (goalAttemptStore == null)
      {
        throw new ArgumentException("GoalAttemptStore cannot be null.");
      }

      if (userStore == null)
      {
        throw new ArgumentException("UserStore cannot be null.");
      }

      if (tokenStore == null)
      {
        throw new ArgumentException("TokenStore cannot be null.");
      }

      if (dateTime == null)
      {
        throw new ArgumentException("DateTimeSource cannot be null.");
      }

      if (log == null)
      {
        throw new ArgumentException("Logger cannot be null.");
      }

      _goalStore = goalStore;
      _goalAttemptStore = goalAttemptStore;
      _userStore = userStore;
      _tokenStore = tokenStore;
      _dateTime = dateTime;
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
        user = await ControllerUtils.GetUserForRequestHeaderTokenAsync(this, _tokenStore, _userStore, _log);
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

      string serializeData = JsonConvert.SerializeObject(goalDatas);

      _log.LogDebug($"Retrieved user's goals {serializeData}.");

      return Ok(serializeData);
    }

    //---------------------------------------------------------------------------------------------

    [HttpPost]
    [Route("api/goals/create")]
    public async Task<IHttpActionResult> CreateGoalAsync(GoalData goalData)
    {
      _log.LogDebug($"Request: {JsonConvert.SerializeObject(goalData)}");

      User user;

      try
      {
        user = await ControllerUtils.GetUserForRequestHeaderTokenAsync(this, _tokenStore, _userStore, _log);
      }
      catch (AuthenticationException ex)
      {
        return BadRequest(ex.Message);
      }
      catch (InternalServerException)
      {
        return InternalServerError();
      }
      
      Goal goal = await _goalStore.CreateGoalAsync(
        user.Id,
        goalData.Name,
        (Goal.TimePeriod)goalData.Period,
        goalData.FrequencyWithinPeriod);

      goalData.Id = goal.Id;

      _log.LogDebug($"Goal created: {goal}");

      return Ok(JsonConvert.SerializeObject(goalData));
    }

    //---------------------------------------------------------------------------------------------

    public async Task<IHttpActionResult> GetPeriodStatsAsync(int goalId)
    {
      _log.LogDebug($"Request: {goalId}");

      User user;

      try
      {
        user = await ControllerUtils.GetUserForRequestHeaderTokenAsync(this, _tokenStore, _userStore, _log);
      }
      catch (AuthenticationException ex)
      {
        return BadRequest(ex.Message);
      }
      catch (InternalServerException)
      {
        return InternalServerError();
      }

      Goal goal = await _goalStore.GetGoalAsync(goalId);

      if (goal == null || goal.UserId != user.Id)
      {
        return NotFound();
      }

      DateTime periodStart;
      DateTime periodEnd;
      GetPeriodBoundsForTime(goal.Period, DateTime.UtcNow, out periodStart, out periodEnd);

      var attempts = await _goalAttemptStore.GetAttemptsAsync(goalId);
      
      var stats = new GoalPeriodStatsData
      {
        GoalId = goal.Id,
        PeriodStart = periodStart,
        PeriodEnd = periodEnd,
        AttemptCount = attempts.Count(),
        TargetAttemptCount = goal.FrequencyWithinPeriod
      };

      return Ok(JsonConvert.SerializeObject(stats));
    }

    //---------------------------------------------------------------------------------------------

    private static void GetPeriodBoundsForTime(Goal.TimePeriod periodType,
                                               DateTime time,
                                               out DateTime periodStart,
                                               out DateTime periodEnd)
    {
      periodStart = new DateTime();
      periodEnd = new DateTime();
    }

    //---------------------------------------------------------------------------------------------
  }
}