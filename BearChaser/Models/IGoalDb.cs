using System.Collections.Generic;
using System.Threading.Tasks;

namespace BearChaser.Models
{
  internal interface IGoalDb
  {
    //---------------------------------------------------------------------------------------------

    void AddGoal(Goal goal);
    void RemoveGoal(Goal goal);
    Task<IEnumerable<Goal>> GetGoalsAsync(int userId);
    Task SaveAsync();

    //---------------------------------------------------------------------------------------------
  }
}