﻿using System;
using System.Threading.Tasks;
using BearChaser.Models;
using BearChaser.Settings;
using BearChaser.Utils;
using BearChaser.Utils.Logging;

namespace BearChaser.Stores
{
  internal class TokenStore : ITokenStore
  {
    //---------------------------------------------------------------------------------------------

    private readonly ITokenDb _tokenDb;
    private readonly IDateTimeSource _dateTimeSource;
    private readonly ILogger _log;
    private readonly int _tokenLifetimeInMinutes;

    //---------------------------------------------------------------------------------------------

    public TokenStore(ITokenDb tokenDb,
                      ITokenSettings settings,
                      IDateTimeSource dateTimeSource,
                      ILogger log)
    {
      _tokenDb = tokenDb;
      _dateTimeSource = dateTimeSource;
      _log = log;
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

      _tokenDb.AddToken(token);

      await SaveToDbAsync();

      _log.LogInfo($"Issued token {token}.");

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
    
    private async Task SaveToDbAsync()
    {
      MarkExpiredTokensForDelete();
      await _tokenDb.SaveAsync();

      _log.LogDebug("TokenStore wrote to db.");
    }

    //---------------------------------------------------------------------------------------------

    private void MarkExpiredTokensForDelete()
    {
      // TODO: User table foreign key needs to be nulled - introduce a SP?
      //DateTime now = _dateTimeSource.Now;

      //foreach (var token in _tokenDb.GetTokens())
      //{
      //  if (token.Expiry > now)
      //  {
      //    continue;
      //  }

      //  _tokenDb.RemoveToken(token);

      //  _log.LogInfo($"Removed expired token {token}.");
      //}
    }

    //---------------------------------------------------------------------------------------------
  }
}