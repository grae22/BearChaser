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
    Task SaveAsync();

    //---------------------------------------------------------------------------------------------
  }
}
