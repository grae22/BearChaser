using System.Collections.Generic;
using System.Threading.Tasks;
using BearChaser.Models;

namespace BearChaser.Db
{
  internal interface IGoalDb
  {
    //---------------------------------------------------------------------------------------------

    void AddGoal(Goal goal);
    void RemoveGoal(Goal goal);
    Task<IEnumerable<Goal>> GetGoalsAsync(int userId);
    Task<Goal> GetGoalAsync(int goalId);
    Task SaveAsync();

    //---------------------------------------------------------------------------------------------
  }
}