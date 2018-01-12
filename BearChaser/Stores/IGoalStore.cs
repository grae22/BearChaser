using System.Collections.Generic;
using System.Threading.Tasks;
using BearChaser.Models;

namespace BearChaser.Stores
{
  public interface IGoalStore
  {
    //---------------------------------------------------------------------------------------------

    Task<Goal> CreateGoalAsync(int userId,
                               string name,
                               Goal.TimePeriod period,
                               int frequencyWithinPeriod);

    Task RemoveGoalAsync(Goal goal);
    Task<IEnumerable<Goal>> GetGoalsAsync(int userId);
    Task<Goal> GetGoalAsync(int goalId);

    //---------------------------------------------------------------------------------------------
  }
}
