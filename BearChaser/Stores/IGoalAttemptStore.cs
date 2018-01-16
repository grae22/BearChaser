using System.Collections.Generic;
using System.Threading.Tasks;
using BearChaser.Models;

namespace BearChaser.Stores
{
  public interface IGoalAttemptStore
  {
    //---------------------------------------------------------------------------------------------

    Task<GoalAttempt> CreateAttemptAsync(int goalId);
    Task RemoveAttemptAsync(int attemptId);
    Task<IEnumerable<GoalAttempt>> GetAttemptsAsync(int goalId);
    Task<GoalAttempt> GetAttemptAsync(int attemptId);

    //---------------------------------------------------------------------------------------------
  }
}
