﻿using System.Threading.Tasks;
using BearChaser.Models;

namespace BearChaser.Stores
{
  public interface IUserStore
  {
    //---------------------------------------------------------------------------------------------

    Task<User> GetUserAsync(string username);
    Task<User> GetUserAsync(Token token);
    Task<User> GetUserAsync(int userId);
    Task AddUserAsync(string username, int passwordHash);
    Task SaveAsync();

    //---------------------------------------------------------------------------------------------
  }
}
