using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Ajax.Utilities;
using BearChaser.Db;
using BearChaser.Models;

namespace BearChaser.Stores
{
  internal class GoalStore : IGoalStore
  {
    //---------------------------------------------------------------------------------------------

    private readonly IGoalDb _goalDb;
    private readonly IUserStore _userStore;

    //---------------------------------------------------------------------------------------------

    public GoalStore(IGoalDb goalDb, IUserStore userStore)
    {
      _goalDb = goalDb;
      _userStore = userStore;
    }

    //---------------------------------------------------------------------------------------------

    public async Task<Goal> CreateGoalAsync(int userId,
                                            string name,
                                            int periodInHours,
                                            int frequencyWithinPeriod,
                                            DateTime startDate)
    {
      await ValidateUserExists(userId);
      ValidateGoalName(name);
      ValidatePeriod(periodInHours);
      ValidateFrequency(frequencyWithinPeriod);

      var goal = new Goal
      {
        Name = name,
        UserId = userId,
        PeriodInHours = periodInHours,
        FrequencyWithinPeriod = frequencyWithinPeriod,
        StartDate = startDate
      };

      _goalDb.AddGoal(goal);

      await _goalDb.SaveAsync();

      return goal;
    }

    //---------------------------------------------------------------------------------------------

    public async Task RemoveGoalAsync(Goal goal)
    {
      _goalDb.RemoveGoal(goal);
      await _goalDb.SaveAsync();
    }

    //---------------------------------------------------------------------------------------------

    public async Task<IEnumerable<Goal>> GetGoalsAsync(int userId)
    {
      return await _goalDb.GetGoalsAsync(userId);
    }

    //---------------------------------------------------------------------------------------------

    public async Task<Goal> GetGoalAsync(int goalId)
    {
      return await _goalDb.GetGoalAsync(goalId);
    }

    //---------------------------------------------------------------------------------------------

    private async Task ValidateUserExists(int userId)
    {
      if (await _userStore.GetUserAsync(userId) == null)
      {
        throw new ArgumentException($"User not found with id {userId}.");
      }
    }

    //---------------------------------------------------------------------------------------------

    private static void ValidateGoalName(string name)
    {
      if (name.IsNullOrWhiteSpace())
      {
        throw new ArgumentException($"Goal name cannot be blank or null.");
      }
    }

    //---------------------------------------------------------------------------------------------

    private static void ValidatePeriod(int periodInHours)
    {
      if (periodInHours > 0)
      {
        return;
      }

      throw new ArgumentException($"Invalid period value {periodInHours}.");
    }

    //---------------------------------------------------------------------------------------------

    private static void ValidateFrequency(int value)
    {
      if (value > 0)
      {
        return;
      }

      throw new ArgumentException($"Frequency cannot be zero or negative, was {value}.");
    }

    //---------------------------------------------------------------------------------------------
  }
}