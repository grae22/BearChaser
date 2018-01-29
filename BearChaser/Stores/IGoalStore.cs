using System;
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
                               int periodInHours,
                               int frequencyWithinPeriod,
                               DateTime startDate);

    Task RemoveGoalAsync(Goal goal);
    Task<IEnumerable<Goal>> GetGoalsAsync(int userId);
    Task<Goal> GetGoalAsync(int goalId);

    //---------------------------------------------------------------------------------------------
  }
}
