using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using BearChaser.Migrations;
using BearChaser.Models;

namespace BearChaser.Db
{
  internal class ApplicationDbContext : DbContext, ITokenDb, IGoalDb, IGoalAttemptDb, IDbQuery
  {
    //---------------------------------------------------------------------------------------------

    public DbSet<Setting> Settings { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Token> Tokens { get; set; }
    public DbSet<Goal> Goals { get; set; }
    public DbSet<GoalAttempt> GoalAttempts { get; set; }

    //---------------------------------------------------------------------------------------------

    public ApplicationDbContext()
    :
      base("DefaultConnection")
    {
      Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext, Configuration>());
    }

    // ITokenDb ===================================================================================

    public void AddToken(Token token)
    {
      Tokens.Add(token);
    }

    //---------------------------------------------------------------------------------------------

    public void RemoveToken(Token token)
    {
      Tokens.Remove(token);
    }

    //---------------------------------------------------------------------------------------------

    public IEnumerable<Token> GetTokens()
    {
      return Tokens;
    }

    //---------------------------------------------------------------------------------------------

    public async Task<Token> GetTokenAsync(Guid guid)
    {
      return await Tokens.FirstOrDefaultAsync(t => t.Value == guid);
    }

    //---------------------------------------------------------------------------------------------

    public async Task RemoveExpiredTokensAsync()
    {
      await Database.ExecuteSqlCommandAsync("EXEC sp_RemoveExpiredTokens");
    }

    // IGoalDb ====================================================================================

    public void AddGoal(Goal goal)
    {
      Goals.Add(goal);
    }

    //---------------------------------------------------------------------------------------------

    public void RemoveGoal(Goal goal)
    {
      Goals.Remove(goal);
    }

    //---------------------------------------------------------------------------------------------

    public async Task<IEnumerable<Goal>> GetGoalsAsync(int userId)
    {
      return await Goals.Where(g => g.UserId == userId).ToListAsync();
    }

    //---------------------------------------------------------------------------------------------

    public async Task<Goal> GetGoalAsync(int goalId)
    {
      return await Goals.FirstOrDefaultAsync(g => g.Id == goalId);
    }

    // IGoalAttemptDb =============================================================================

    public void AddAttempt(GoalAttempt goal)
    {
      GoalAttempts.Add(goal);
    }

    //---------------------------------------------------------------------------------------------

    public async Task RemoveAttempt(int attemptId)
    {
      GoalAttempt attempt = await GoalAttempts.FirstOrDefaultAsync(a => a.Id == attemptId);

      if (attempt == null)
      {
        return;
      }

      GoalAttempts.Remove(attempt);
    }

    //---------------------------------------------------------------------------------------------

    public IQueryable<GoalAttempt> GetAttempts(int goalId)
    {
      return GoalAttempts.Where(g => g.GoalId == goalId);
    }

    //---------------------------------------------------------------------------------------------

    public async Task<GoalAttempt> GetAttemptAsync(int attemptId)
    {
      return await GoalAttempts.FirstOrDefaultAsync(a => a.Id == attemptId);
    }

    // IDbQuery ===================================================================================

    public async Task<List<T>> ExecuteSql<T>(string sql)
    {
      return await Database.SqlQuery<T>(sql).ToListAsync();
    }

    // Common =====================================================================================

    public async Task SaveAsync()
    {
      await SaveChangesAsync();
    }

    //---------------------------------------------------------------------------------------------
  }
}