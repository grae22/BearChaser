using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BearChaser.Db;
using BearChaser.Models;
using BearChaser.Utils;

namespace BearChaser.Stores
{
  internal class GoalAttemptStore : IGoalAttemptStore
  {
    //---------------------------------------------------------------------------------------------

    private readonly IGoalAttemptDb _goalAttemptDb;
    private readonly IGoalStore _goalStore;
    private readonly IDateTimeSource _dateTimeSource;

    //---------------------------------------------------------------------------------------------

    public GoalAttemptStore(IGoalAttemptDb goalAttemptDb,
                            IGoalStore goalStore,
                            IDateTimeSource dateTimeSource)
    {
      _goalAttemptDb = goalAttemptDb;
      _goalStore = goalStore;
      _dateTimeSource = dateTimeSource;
    }

    //---------------------------------------------------------------------------------------------

    public async Task<GoalAttempt> CreateAttemptAsync(int goalId)
    {
      await ValidateGoalExists(goalId);

      var attempt = new GoalAttempt
      {
        GoalId = goalId,
        Timestamp = _dateTimeSource.Now
      };

      _goalAttemptDb.AddAttempt(attempt);

      await _goalAttemptDb.SaveAsync();

      return attempt;
    }

    //---------------------------------------------------------------------------------------------

    public async Task RemoveAttemptAsync(int attemptId)
    {
      await _goalAttemptDb.RemoveAttempt(attemptId);
      await _goalAttemptDb.SaveAsync();
    }

    //---------------------------------------------------------------------------------------------

    public async Task<IEnumerable<GoalAttempt>> GetAttemptsAsync(int goalId)
    {
      return await _goalAttemptDb.GetAttemptsAsync(goalId);
    }

    //---------------------------------------------------------------------------------------------

    public async Task<GoalAttempt> GetAttemptAsync(int attemptId)
    {
      return await _goalAttemptDb.GetAttemptAsync(attemptId);
    }

    //---------------------------------------------------------------------------------------------

    private async Task ValidateGoalExists(int goalId)
    {
      if (await _goalStore.GetGoalAsync(goalId) == null)
      {
        throw new ArgumentException($"Goal not found with id {goalId}.");
      }
    }

    //---------------------------------------------------------------------------------------------
  }
}