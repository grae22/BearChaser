using System.Linq;
using System.Threading.Tasks;
using BearChaser.Models;

namespace BearChaser.Db
{
  internal interface IGoalAttemptDb
  {
    //---------------------------------------------------------------------------------------------

    void AddAttempt(GoalAttempt goal);
    Task RemoveAttempt(int attemptId);
    IQueryable<GoalAttempt> GetAttempts(int goalId);
    Task<GoalAttempt> GetAttemptAsync(int attemptId);
    Task SaveAsync();

    //---------------------------------------------------------------------------------------------
  }
}