using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BearChaser.Models;

namespace BearChaser.Db
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
