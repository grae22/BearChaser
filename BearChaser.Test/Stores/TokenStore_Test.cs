using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using NSubstitute;
using BearChaser.Models;
using BearChaser.Settings;
using BearChaser.Stores;
using BearChaser.Utils;
using BearChaser.Utils.Logging;

namespace BearChaser.Test.Stores
{
  [TestFixture]
  [Category("TokenStore")]
  public class TokenStore_Test
  {
    //---------------------------------------------------------------------------------------------

    [Test]
    public async Task GetNewTokenAsync_GivenNothing_ShouldReturnValidGuid()
    {
      // Arrange.
      var tokensDb = Substitute.For<ITokenDb>();
      var settings = Substitute.For<ITokenSettings>();
      var dateTimeSource = Substitute.For<IDateTimeSource>();
      var log = Substitute.For<ILogger>();
      var testObject = new TokenStore(tokensDb, settings, dateTimeSource, log);

      // Act.
      Token token = await testObject.GetNewTokenAsync();

      // Assert.
      Assert.NotNull(token);
      Assert.AreNotEqual(Guid.Empty, token);
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public async Task GetNewTokenAsync_GivenTokenLifetimeOfFiveMins_ShouldReturnTokenWithFutureExpiryDate()
    {
      // Arrange.
      var tokensDb = Substitute.For<ITokenDb>();

      var settings = Substitute.For<ITokenSettings>();
      settings.TokenLifetimeInMinutes.Returns(5);

      var dateTimeSource = Substitute.For<IDateTimeSource>();
      dateTimeSource.Now.Returns(DateTime.Now);

      var log = Substitute.For<ILogger>();
      var dbSaveMethod = new Func<Task>(async () => await Task.Delay(0));
      var testObject = new TokenStore(tokensDb, settings, dateTimeSource, log);

      // Act.
      Token token = await testObject.GetNewTokenAsync();

      // Assert.
      Assert.True(token.Expiry > dateTimeSource.Now);
      Assert.AreEqual(dateTimeSource.Now.AddMinutes(5), token.Expiry);
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public async Task GetNewTokenAsync_GivenNothing_ShouldSaveNewTokenToDb()
    {
      // Arrange.
      var tokensDb = Substitute.For<ITokenDb>();
      var settings = Substitute.For<ITokenSettings>();
      var dateTimeSource = Substitute.For<IDateTimeSource>();
      var log = Substitute.For<ILogger>();
      var testObject = new TokenStore(tokensDb, settings, dateTimeSource, log);

      // Act.
      Token token = await testObject.GetNewTokenAsync();

      // Assert.
      tokensDb.Received().AddToken(token);
      await tokensDb.Received().SaveAsync();
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public void IsTokenValid_GivenUnexpiredToken_ShouldReturnTrue()
    {
      // Arrange.
      var tokensDb = Substitute.For<ITokenDb>();
      var settings = Substitute.For<ITokenSettings>();
      var dateTimeSource = Substitute.For<IDateTimeSource>();
      var log = Substitute.For<ILogger>();
      var testObject = new TokenStore(tokensDb, settings, dateTimeSource, log);

      var token = new Token
      {
        Expiry = dateTimeSource.Now.AddMilliseconds(1)
      };

      // Act.
      bool isValid = testObject.IsTokenValid(token);

      // Assert.
      Assert.True(isValid);
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public void IsTokenValid_GivenExpiredToken_ShouldReturnFalse()
    {
      // Arrange.
      var tokensDb = Substitute.For<ITokenDb>();
      var settings = Substitute.For<ITokenSettings>();
      var dateTimeSource = Substitute.For<IDateTimeSource>();
      var log = Substitute.For<ILogger>();
      var testObject = new TokenStore(tokensDb, settings, dateTimeSource, log);

      dateTimeSource.Now.Returns(DateTime.Now);

      var token = new Token
      {
        Expiry = dateTimeSource.Now.AddMilliseconds(-1)
      };

      // Act.
      bool isValid = testObject.IsTokenValid(token);

      // Assert.
      Assert.False(isValid);
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public void IsTokenValid_GivenNullToken_ShouldReturnFalse()
    {
      // Arrange.
      var tokensDb = Substitute.For<ITokenDb>();
      var settings = Substitute.For<ITokenSettings>();
      var dateTimeSource = Substitute.For<IDateTimeSource>();
      var log = Substitute.For<ILogger>();
      var testObject = new TokenStore(tokensDb, settings, dateTimeSource, log);

      // Act.
      bool isValid = testObject.IsTokenValid(null);

      // Assert.
      Assert.False(isValid);
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    [Ignore("Pending update to null the user table's token foreign key.")]
    public async Task DeleteExpiredTokens_GivenExpiredAndValidTokens_ShouldDeleteTheExpiredTokens()
    {
      // Arrange.
      var tokensDb = Substitute.For<ITokenDb>();
      var settings = Substitute.For<ITokenSettings>();
      var dateTimeSource = Substitute.For<IDateTimeSource>();
      var log = Substitute.For<ILogger>();
      var testObject = new TokenStore(tokensDb, settings, dateTimeSource, log);

      dateTimeSource.Now.Returns(DateTime.Now);

      var validToken = new Token { Expiry = dateTimeSource.Now.AddMilliseconds(1) };
      var expiredToken1 = new Token { Expiry = dateTimeSource.Now.AddMilliseconds(-1) };
      var expiredToken2 = new Token { Expiry = dateTimeSource.Now };

      tokensDb.GetTokens().GetEnumerator().Returns(
        new List<Token>
        {
          expiredToken2,
          validToken,
          expiredToken1
        }
        .GetEnumerator());
      
      // Act.
      await testObject.GetNewTokenAsync();

      // Assert.
      tokensDb.Received(0).RemoveToken(validToken);
      tokensDb.Received(1).RemoveToken(expiredToken1);
      tokensDb.Received(1).RemoveToken(expiredToken2);
    }

    //---------------------------------------------------------------------------------------------
  }
}
