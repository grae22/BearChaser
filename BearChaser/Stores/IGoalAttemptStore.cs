using System.Collections.Generic;
using System.Threading.Tasks;
using BearChaser.Models;

namespace BearChaser.Stores
{
  public interface IGoalAttemptStore
  {
    //---------------------------------------------------------------------------------------------

    Task<GoalAttempt> CreateAttemptAsync(int goalId);
    Task RemoveAttemptAsync(GoalAttempt attempt);
    Task<IEnumerable<GoalAttempt>> GetAttemptsAsync(int goalId);

    //---------------------------------------------------------------------------------------------
  }
}
