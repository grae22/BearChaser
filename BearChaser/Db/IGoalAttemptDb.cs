using System.Collections.Generic;
using System.Threading.Tasks;
using BearChaser.Models;

namespace BearChaser.Db
{
  internal interface IGoalAttemptDb
  {
    //---------------------------------------------------------------------------------------------

    void AddAttempt(GoalAttempt goal);
    void RemoveAttempt(GoalAttempt goal);
    Task<IEnumerable<GoalAttempt>> GetAttemptsAsync(int goalId);
    Task SaveAsync();

    //---------------------------------------------------------------------------------------------
  }
}