using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BearChaser.Models
{
  internal interface ITokenDb
  {
    //---------------------------------------------------------------------------------------------

    void AddToken(Token token);
    void RemoveToken(Token token);
    IEnumerable<Token> GetTokens();
    Task<Token> GetTokenAsync(Guid guid);
    Task RemoveExpiredTokensAsync();
    Task SaveAsync();

    //---------------------------------------------------------------------------------------------
  }
}
