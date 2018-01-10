using System;
using System.Threading.Tasks;
using BearChaser.Models;

namespace BearChaser.Stores
{
  public interface ITokenStore
  {
    //---------------------------------------------------------------------------------------------

    Task<Token> GetNewTokenAsync();
    Task<Token> GetExistingTokenByGuidAsync(Guid guid);
    bool IsTokenValid(Token token);

    //---------------------------------------------------------------------------------------------
  }
}
