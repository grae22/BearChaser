// TODO: Something needs to clear out expired tokens from time to time.

using System;
using System.Data.Entity;
using System.Threading.Tasks;
using BearChaser.Models;
using BearChaser.Settings;
using BearChaser.Utils;

namespace BearChaser.Stores
{
  internal class TokenStore : ITokenStore
  {
    //---------------------------------------------------------------------------------------------

    private readonly IDbSet<Token> _tokens;
    private readonly IDateTimeSource _dateTimeSource;
    private readonly Func<Task> _saveAction;
    private readonly int _tokenLifetimeInMinutes;

    //---------------------------------------------------------------------------------------------

    public TokenStore(IDbSet<Token> tokens,
                      ITokenSettings settings,
                      IDateTimeSource dateTimeSource,
                      Func<Task> saveAction)
    {
      _tokens = tokens;
      _dateTimeSource = dateTimeSource;
      _saveAction = saveAction;
      _tokenLifetimeInMinutes = settings.TokenLifetimeInMinutes;
    }

    //---------------------------------------------------------------------------------------------

    public async Task<Token> GetNewTokenAsync()
    {
      var token = new Token
      {
        Value = Guid.NewGuid(),
        Expiry = _dateTimeSource.Now.AddMinutes(_tokenLifetimeInMinutes)
      };

      _tokens.Add(token);

      await SaveToDbAsync();

      return token;
    }

    //---------------------------------------------------------------------------------------------

    public bool IsTokenValid(Token token)
    {
      // TODO: It may be desirable to check that the token is actually present in the db.

      if (token == null)
      {
        return false;
      }

      return token.Expiry > _dateTimeSource.Now;
    }

    //---------------------------------------------------------------------------------------------
    
    private Task SaveToDbAsync()
    {
      return _saveAction.Invoke();
    }

    //---------------------------------------------------------------------------------------------
  }
}