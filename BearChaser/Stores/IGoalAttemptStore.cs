using System.Linq;
using System.Threading.Tasks;
using BearChaser.Models;

namespace BearChaser.Stores
{
  public interface IGoalAttemptStore
  {
    //---------------------------------------------------------------------------------------------

    Task<GoalAttempt> CreateAttemptAsync(int goalId);
    Task RemoveAttemptAsync(int attemptId);
    IQueryable<GoalAttempt> GetAttempts(int goalId);
    Task<GoalAttempt> GetAttemptAsync(int attemptId);

    //---------------------------------------------------------------------------------------------
  }
}
