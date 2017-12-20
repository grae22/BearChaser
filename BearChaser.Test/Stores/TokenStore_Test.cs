using System;
using System.Data.Entity;
using System.Threading.Tasks;
using NUnit.Framework;
using NSubstitute;
using BearChaser.Models;
using BearChaser.Settings;
using BearChaser.Stores;
using BearChaser.Utils;

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
      var tokensInDb = Substitute.For<IDbSet<Token>>();
      var settings = Substitute.For<ITokenSettings>();
      var dateTimeSource = Substitute.For<IDateTimeSource>();
      var dbSaveMethod = new Func<Task>(async () => await Task.Delay(0));
      var testObject = new TokenStore(tokensInDb, settings, dateTimeSource, dbSaveMethod);

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
      var tokensInDb = Substitute.For<IDbSet<Token>>();

      var settings = Substitute.For<ITokenSettings>();
      settings.TokenLifetimeInMinutes.Returns(5);

      var dateTimeSource = Substitute.For<IDateTimeSource>();
      dateTimeSource.Now.Returns(DateTime.Now);

      var dbSaveMethod = new Func<Task>(async () => await Task.Delay(0));
      var testObject = new TokenStore(tokensInDb, settings, dateTimeSource, dbSaveMethod);

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
      var tokensInDb = Substitute.For<IDbSet<Token>>();
      var settings = Substitute.For<ITokenSettings>();
      var dateTimeSource = Substitute.For<IDateTimeSource>();
      var dbSaveCalled = false;
      var dbSaveMethod = new Func<Task>(async () => { dbSaveCalled = true; await Task.Delay(0); });
      var testObject = new TokenStore(tokensInDb, settings, dateTimeSource, dbSaveMethod);

      // Act.
      Token token = await testObject.GetNewTokenAsync();

      // Assert.
      tokensInDb.Received().Add(token);

      Assert.True(dbSaveCalled);
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public void IsTokenValid_GivenUnexpiredToken_ShouldReturnTrue()
    {
      // Arrange.
      var tokensInDb = Substitute.For<IDbSet<Token>>();
      var settings = Substitute.For<ITokenSettings>();
      var dateTimeSource = Substitute.For<IDateTimeSource>();
      var dbSaveMethod = new Func<Task>(async () => await Task.Delay(0));
      var testObject = new TokenStore(tokensInDb, settings, dateTimeSource, dbSaveMethod);

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
      var tokensInDb = Substitute.For<IDbSet<Token>>();
      var settings = Substitute.For<ITokenSettings>();
      var dateTimeSource = Substitute.For<IDateTimeSource>();
      var dbSaveMethod = new Func<Task>(async () => await Task.Delay(0));
      var testObject = new TokenStore(tokensInDb, settings, dateTimeSource, dbSaveMethod);

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
      var tokensInDb = Substitute.For<IDbSet<Token>>();
      var settings = Substitute.For<ITokenSettings>();
      var dateTimeSource = Substitute.For<IDateTimeSource>();
      var dbSaveMethod = new Func<Task>(async () => await Task.Delay(0));
      var testObject = new TokenStore(tokensInDb, settings, dateTimeSource, dbSaveMethod);

      // Act.
      bool isValid = testObject.IsTokenValid(null);

      // Assert.
      Assert.False(isValid);
    }

    //---------------------------------------------------------------------------------------------
  }
}
