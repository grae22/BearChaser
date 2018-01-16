using System.Collections.Generic;
using System.Threading.Tasks;
using BearChaser.Models;

namespace BearChaser.Db
{
  internal interface IGoalAttemptDb
  {
    //---------------------------------------------------------------------------------------------

    void AddAttempt(GoalAttempt goal);
    Task RemoveAttempt(int attemptId);
    Task<IEnumerable<GoalAttempt>> GetAttemptsAsync(int goalId);
    Task<GoalAttempt> GetAttemptAsync(int attemptId);
    Task SaveAsync();

    //---------------------------------------------------------------------------------------------
  }
}