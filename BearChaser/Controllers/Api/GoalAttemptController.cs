using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Authentication;
using System.Threading.Tasks;
using System.Web.Http;
using AutoMapper;
using BearChaser.Controllers.Utils;
using BearChaser.DataTransferObjects;
using BearChaser.Exceptions;
using BearChaser.Models;
using BearChaser.Stores;
using BearChaser.Utils.Logging;
using Newtonsoft.Json;
using WebGrease.Css.Extensions;

namespace BearChaser.Controllers.Api
{
  public class GoalAttemptController : ApiController
  {
    //---------------------------------------------------------------------------------------------

    private readonly IGoalAttemptStore _attemptStore;
    private readonly IGoalStore _goalStore;
    private readonly ITokenStore _tokenStore;
    private readonly IUserStore _userStore;
    private readonly ILogger _log;

    //---------------------------------------------------------------------------------------------

    public GoalAttemptController(IGoalAttemptStore attemptStore,
                                 IGoalStore goalStore,
                                 ITokenStore tokenStore,
                                 IUserStore userStore,
                                 ILogger log)
    {
      if (attemptStore == null)
      {
        throw new ArgumentNullException(nameof(attemptStore));
      }

      if (goalStore == null)
      {
        throw new ArgumentNullException(nameof(goalStore));
      }

      if (tokenStore == null)
      {
        throw new ArgumentNullException(nameof(tokenStore));
      }

      if (userStore == null)
      {
        throw new ArgumentNullException(nameof(userStore));
      }

      if (log == null)
      {
        throw new ArgumentNullException(nameof(log));
      }

      _attemptStore = attemptStore;
      _goalStore = goalStore;
      _tokenStore = tokenStore;
      _userStore = userStore;
      _log = log;
    }

    //---------------------------------------------------------------------------------------------

    [HttpPost]
    [Route("api/goalAttempts/create")]
    public async Task<IHttpActionResult> CreateAttemptAsync(GoalAttemptData attempt)
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

      Goal goal = await _goalStore.GetGoalAsync(attempt.GoalId);

      if (goal == null || goal.UserId != user.Id)
      {
        _log.LogDebug($"Goal not found with id {attempt.GoalId} for user {user.Id}.");
        return NotFound();
      }

      GoalAttempt newAttempt = await _attemptStore.CreateAttemptAsync(attempt.GoalId);

      _log.LogInfo($"Created goal-attempt for user {user.Id} : {newAttempt}");

      return StatusCode(HttpStatusCode.Created);
    }

    //---------------------------------------------------------------------------------------------

    [HttpDelete]
    [Route("api/goalAttempts")]
    public async Task<IHttpActionResult> DeleteAttemptAsync(int attemptId)
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

      GoalAttempt attempt = await _attemptStore.GetAttemptAsync(attemptId);

      if (attempt == null)
      {
        _log.LogDebug($"Goal-attempt not found with id {attemptId} for user {user.Id}.");
        return NotFound();
      }

      Goal goal = await _goalStore.GetGoalAsync(attempt.GoalId);

      if (goal == null || goal.UserId != user.Id)
      {
        _log.LogDebug($"Goal not found with id {attempt.GoalId} for user {user.Id}.");
        return NotFound();
      }

      await _attemptStore.RemoveAttemptAsync(attemptId);

      _log.LogInfo($"Created goal-attempt {attemptId} for user {user.Id}.");

      return Ok();
    }

    //---------------------------------------------------------------------------------------------

    [HttpGet]
    [Route("api/goalAttempts")]
    public async Task<IHttpActionResult> GetAttemptsAsync(int goalId)
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

      Goal goal = await _goalStore.GetGoalAsync(goalId);

      if (goal == null || goal.UserId != user.Id)
      {
        _log.LogDebug($"Goal not found with id {goalId} for user {user.Id}.");
        return NotFound();
      }

      var attempts = await _attemptStore.GetAttemptsAsync(goalId);

      var attemptDatas = new List<GoalAttemptData>();
      attempts.ForEach(a => attemptDatas.Add(Mapper.Map<GoalAttemptData>(a)));

      string serialisedAttempts = JsonConvert.SerializeObject(attemptDatas);

      _log.LogDebug($"Found attempts for goal id {goalId} for user {user.Id} : {serialisedAttempts}");
      
      return Ok(serialisedAttempts);
    }

    //---------------------------------------------------------------------------------------------
  }
}