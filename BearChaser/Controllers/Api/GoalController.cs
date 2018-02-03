using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using System.Web.Http;
using AutoMapper;
using Newtonsoft.Json;
using WebGrease.Css.Extensions;
using BearChaser.Controllers.Utils;
using BearChaser.DataTransferObjects;
using BearChaser.Db;
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
    private readonly IDbQuery _dbQuery;
    private readonly IDateTimeSource _dateTime;
    private readonly ILogger _log;

    //---------------------------------------------------------------------------------------------

    public GoalController(IGoalStore goalStore,
                          IGoalAttemptStore goalAttemptStore,
                          IUserStore userStore,
                          ITokenStore tokenStore,
                          IDbQuery dbQuery,
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

      if (dbQuery == null)
      {
        throw new ArgumentException("DbQuery cannot be null.");
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
      _dbQuery = dbQuery;
      _dateTime = dateTime;
      _log = log;
    }

    //---------------------------------------------------------------------------------------------

    // GET: api/goals

    [HttpGet]
    [Route("api/goals")]
    public async Task<IHttpActionResult> GetGoalsAsync()
    {
      _log.LogDebug("Request received.");

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

      var usersGoals = await _goalStore.GetGoalsAsync(user.Id);

      var goalDatas = new List<GoalData>();
      usersGoals.ForEach(g => goalDatas.Add(Mapper.Map<GoalData>(g)));

      goalDatas.Sort((g1, g2) => string.Compare(g1.Name, g2.Name));

      string serializeData = JsonConvert.SerializeObject(goalDatas);

      _log.LogDebug($"Returning goals {serializeData}.");

      return Ok(serializeData);
    }

    //---------------------------------------------------------------------------------------------

    // POST: api/goals

    [HttpPost]
    [Route("api/goals")]
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
        goalData.PeriodInHours,
        goalData.FrequencyWithinPeriod,
        goalData.StartDate);

      _log.LogDebug($"Goal created: {goal}");

      return Ok(
        JsonConvert.SerializeObject(
          Mapper.Map<GoalData>(goal)));
    }

    //---------------------------------------------------------------------------------------------

    // GET: api/goals/periodStats?goalId=123

    [HttpGet]
    [Route("api/goals/periodStats")]
    public async Task<IHttpActionResult> GetPeriodStatsAsync(int goalId)
    {
      _log.LogDebug($"Request: goalId={goalId}");

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
        _log.LogDebug($"Goal {goalId} not found for user {user.Id}.");
        return NotFound();
      }

      GetPeriodBoundsForTime(goal, _dateTime.Now, out DateTime periodStart, out DateTime periodEnd);

      List<GoalAttempt> attemptsInDateOrder = await GetAttemptsForPeriod(goalId, periodStart, periodEnd);
      attemptsInDateOrder.Sort((a1, a2) => a1.Timestamp.CompareTo(a2.Timestamp)); 

      var stats = new GoalPeriodStatsData
      {
        GoalId = goal.Id,
        PeriodStart = periodStart,
        PeriodEnd = periodEnd,
        AttemptCount = attemptsInDateOrder.Count,
        LastAttemptDate = attemptsInDateOrder.Any() ? (DateTime?)attemptsInDateOrder[attemptsInDateOrder.Count-1].Timestamp : null,
        TargetAttemptCount = goal.FrequencyWithinPeriod
      };

      await PopulateWithGoalCompletionStats(goalId, stats);

      _log.LogDebug($"Retrieved stats for user {user.Id}, goal {goalId} : {JsonConvert.SerializeObject(stats)}");

      return Ok(JsonConvert.SerializeObject(stats));
    }

    //---------------------------------------------------------------------------------------------

    private static void GetPeriodBoundsForTime(Goal goal,
                                               DateTime time,
                                               out DateTime periodStart,
                                               out DateTime periodEnd)
    {
      double hoursSinceStart = (time - goal.StartDate).TotalHours;
      int periodsSinceStart = (int)(hoursSinceStart / goal.PeriodInHours);

      periodStart = goal.StartDate.AddHours(periodsSinceStart * goal.PeriodInHours);
      periodEnd = goal.StartDate.AddHours((periodsSinceStart + 1) * goal.PeriodInHours).AddSeconds(-1);
    }


    //---------------------------------------------------------------------------------------------

    private async Task<List<GoalAttempt>> GetAttemptsForPeriod(int goalId,
                                                               DateTime periodStart,
                                                               DateTime periodEnd)
    {
      return await
        _goalAttemptStore
          .GetAttempts(goalId)
          .Where(a => a.Timestamp >= periodStart && a.Timestamp <= periodEnd)
          .ToListAsync();
    }

    //---------------------------------------------------------------------------------------------

    private async Task PopulateWithGoalCompletionStats(int goalId, GoalPeriodStatsData stats)
    {
      // All periods.
      var results = await _dbQuery.ExecuteSql<int>($"EXEC dbo.sp_CalculateGoalAverageCompletionAcrossAllPeriods {goalId}");

      if (results.Count > 0)
      {
        stats.AverageCompletionAcrossAllPeriods = results[0];
      }
      else
      {
        _log.LogError($"Failed to retrieve goal average completion across all periods stats for goal {goalId}.");
      }

      // Last 3 periods.
      results = await _dbQuery.ExecuteSql<int>($"EXEC dbo.sp_CalculateGoalAverageCompletionAcrossLast3Periods {goalId}");

      if (results.Count > 0)
      {
        stats.AverageCompletionAcrossLast3Periods = results[0];
      }
      else
      {
        _log.LogError($"Failed to retrieve goal average completion across last 3 periods stats for goal {goalId}.");
      }
    }

    //---------------------------------------------------------------------------------------------
  }
}