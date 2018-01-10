using System;
using System.Data.Entity;
using System.Threading.Tasks;
using BearChaser.Models;

namespace BearChaser.Stores
{
  public class UserStore : IUserStore
  {
    //---------------------------------------------------------------------------------------------

    private readonly ApplicationDbContext _dbContext = new ApplicationDbContext();

    //---------------------------------------------------------------------------------------------

    public async Task<User> GetUserAsync(string username)
    {
      return await _dbContext.Users.FirstOrDefaultAsync(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
    }

    //---------------------------------------------------------------------------------------------

    public async Task<User> GetUserAsync(Token token)
    {
      return await _dbContext.Users.FirstOrDefaultAsync(u => u.Token.Value == token.Value);
    }

    //---------------------------------------------------------------------------------------------

    public async Task AddUserAsync(string username, int passwordHash)
    {
      User user = await GetUserAsync(username);

      if (user != null)
      {
        throw new ArgumentException($"User already exists with username '{username}'.");
      }

      user = new User
      {
        Username = username,
        Password = passwordHash,
        Token = null
      };

      _dbContext.Users.Add(user);

      await SaveAsync();
    }

    //---------------------------------------------------------------------------------------------

    public async Task SaveAsync()
    {
      await _dbContext.SaveChangesAsync();
    }

    //---------------------------------------------------------------------------------------------
  }
}