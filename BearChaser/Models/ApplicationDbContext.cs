using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace BearChaser.Models
{
  internal class ApplicationDbContext : DbContext, ITokenDb, IGoalDb
  {
    //---------------------------------------------------------------------------------------------

    public DbSet<Setting> Settings { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Token> Tokens { get; set; }
    public DbSet<Goal> Goals { get; set; }

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

    // Common =====================================================================================

    public async Task SaveAsync()
    {
      await SaveChangesAsync();
    }

    //---------------------------------------------------------------------------------------------
  }
}