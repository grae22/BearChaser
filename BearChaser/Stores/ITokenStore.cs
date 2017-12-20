using System.Threading.Tasks;
using BearChaser.Models;

namespace BearChaser.Stores
{
  public interface ITokenStore
  {
    //---------------------------------------------------------------------------------------------

    Task<Token> GetNewTokenAsync();
    bool IsTokenValid(Token token);

    //---------------------------------------------------------------------------------------------
  }
}
