using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using NUnit.Framework;
using NSubstitute;
using BearChaser.Controllers;
using BearChaser.Models;
using BearChaser.Settings;
using BearChaser.Stores;
using BearChaser.Utils;
using BearChaser.Utils.Logging;

namespace BearChaser.Test.Controllers
{
  [TestFixture]
  [Category("UserController")]
  public class UserController_Test
  {
    //---------------------------------------------------------------------------------------------

    [Test]
    public async Task Register_GivenBlankUsername_ShouldReturnBadRequest()
    {
      // Arrange.
      var userStore = Substitute.For<IUserStore>();
      var tokenStore = Substitute.For<ITokenStore>();
      var userSettings = CreateUserSettings();
      var log = Substitute.For<ILogger>();
      var testObject = new UserController(userStore, tokenStore, userSettings, log);

      // Act.
      ActionResult result = await testObject.Register("  ", "password");

      var httpStatusCodeResult = result as HttpStatusCodeResult;

      // Assert.
      Assert.NotNull(httpStatusCodeResult);
      Assert.AreEqual((int)HttpStatusCode.BadRequest, httpStatusCodeResult.StatusCode);
      Assert.AreEqual("Username can't be blank.", httpStatusCodeResult.StatusDescription);
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public async Task Register_GivenUsernameThatAlreadyExists_ShouldReturnBadRequest()
    {
      // Arrange.
      var userStore = Substitute.For<IUserStore>();
      userStore.GetUserAsync("username").Returns(new User());

      var tokenStore = Substitute.For<ITokenStore>();

      var userSettings = CreateUserSettings();
      var log = Substitute.For<ILogger>();

      var testObject = new UserController(userStore, tokenStore, userSettings, log);

      // Act.
      ActionResult result = await testObject.Register("username", "password");

      var httpStatusCodeResult = result as HttpStatusCodeResult;

      // Assert.
      Assert.NotNull(httpStatusCodeResult);
      Assert.AreEqual((int)HttpStatusCode.BadRequest, httpStatusCodeResult.StatusCode);
      Assert.AreEqual("Username already exists.", httpStatusCodeResult.StatusDescription);
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public async Task Register_GivenShortPassword_ShouldReturnBadRequest()
    {
      // Arrange.
      var userStore = Substitute.For<IUserStore>();
      userStore.GetUserAsync(Arg.Any<string>()).Returns((User)null);

      var tokenStore = Substitute.For<ITokenStore>();

      var userSettings = CreateUserSettings();
      var log = Substitute.For<ILogger>();

      var testObject = new UserController(userStore, tokenStore, userSettings, log);

      // Act.
      ActionResult result = await testObject.Register("username", "1234567");

      var httpStatusCodeResult = result as HttpStatusCodeResult;

      // Assert.
      Assert.NotNull(httpStatusCodeResult);
      Assert.AreEqual((int)HttpStatusCode.BadRequest, httpStatusCodeResult.StatusCode);
      Assert.AreEqual("Password must be 8 or more characters long.", httpStatusCodeResult.StatusDescription);
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public async Task Register_GivenSuccessfulRegistration_ShouldReturnOk()
    {
      // Arrange.
      var userStore = Substitute.For<IUserStore>();
      userStore.GetUserAsync(Arg.Any<string>()).Returns((User)null);

      var tokenStore = Substitute.For<ITokenStore>();

      var userSettings = CreateUserSettings();
      var log = Substitute.For<ILogger>();

      var testObject = new UserController(userStore, tokenStore, userSettings, log);

      // Act.
      ActionResult result = await testObject.Register("username", "12345678");

      var httpStatusCodeResult = result as HttpStatusCodeResult;

      // Assert.
      Assert.NotNull(httpStatusCodeResult);
      Assert.AreEqual((int)HttpStatusCode.OK, httpStatusCodeResult.StatusCode);
      Assert.AreEqual("User registered.", httpStatusCodeResult.StatusDescription);
    }

    //---------------------------------------------------------------------------------------------
    
    [Test]
    public async Task Login_GivenValidUsernameAndPassword_ShouldReturnToken()
    {
      // Arrange.
      var tokenValue = Guid.NewGuid();
      
      var user = new User
      {
        Password = "CorrectPassword".GetAsPasswordHash(),
        Token = new Token { Value = tokenValue }
      };

      var userStore = Substitute.For<IUserStore>();
      userStore.GetUserAsync(Arg.Any<string>()).Returns(user);

      var tokenStore = Substitute.For<ITokenStore>();
      tokenStore.IsTokenValid(Arg.Any<Token>()).Returns(true);

      var userSettings = CreateUserSettings();
      var log = Substitute.For<ILogger>();

      var testObject = new UserController(userStore, tokenStore, userSettings, log);

      // Act.
      ActionResult result = await testObject.Login("username", "CorrectPassword");

      var jsonResult = result as JsonResult;
      var returnedToken = JsonConvert.DeserializeObject<Guid>(jsonResult.Data as string);

      // Assert.
      Assert.AreEqual(tokenValue, returnedToken);
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public async Task Login_GivenUnknownUsername_ShouldReturnNotFound()
    {
      // Arrange.
      var userStore = Substitute.For<IUserStore>();
      userStore.GetUserAsync(Arg.Any<string>()).Returns((User)null);

      var tokenStore = Substitute.For<ITokenStore>();

      var userSettings = CreateUserSettings();
      var log = Substitute.For<ILogger>();

      var testObject = new UserController(userStore, tokenStore, userSettings, log);

      // Act.
      ActionResult result = await testObject.Login("username", string.Empty);

      // Assert.
      var httpNotFoundResult = result as HttpNotFoundResult;

      Assert.NotNull(httpNotFoundResult);
      StringAssert.Contains("username", httpNotFoundResult.StatusDescription);
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public async Task Login_GivenIncorrectPassword_ShouldReturnAuthenticationError()
    {
      // Arrange.
      var user = new User
      {
        Password = "CorrectPassword".GetAsPasswordHash()
      };

      var userStore = Substitute.For<IUserStore>();
      userStore.GetUserAsync(Arg.Any<string>()).Returns(user);

      var tokenStore = Substitute.For<ITokenStore>();

      var userSettings = CreateUserSettings();
      var log = Substitute.For<ILogger>();

      var testObject = new UserController(userStore, tokenStore, userSettings, log);

      // Act.
      ActionResult result = await testObject.Login("username", "IncorrectPassword");

      // Assert.
      var httpStatusCodeResult = result as HttpStatusCodeResult;

      Assert.NotNull(httpStatusCodeResult);
      Assert.AreEqual((int)HttpStatusCode.Forbidden, httpStatusCodeResult.StatusCode);
      Assert.AreEqual("Invalid password.", httpStatusCodeResult.StatusDescription);
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public async Task Login_GivenNoCurrentToken_ShouldAllocateUserNewToken()
    {
      // Arrange.
      var user = new User
      {
        Password = "CorrectPassword".GetAsPasswordHash()
      };

      var userStore = Substitute.For<IUserStore>();
      userStore.GetUserAsync(Arg.Any<string>()).Returns(user);

      var tokenStore = Substitute.For<ITokenStore>();
      tokenStore.GetNewTokenAsync().Returns(new Token());

      var userSettings = CreateUserSettings();
      var log = Substitute.For<ILogger>();

      var testObject = new UserController(userStore, tokenStore, userSettings, log);

      // Act.
      await testObject.Login("username", "CorrectPassword");

      // Assert.
      Assert.NotNull(user.Token);
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public async Task Login_GivenHasCurrentUnexpiredToken_ShouldReturnSameToken()
    {
      // Arrange.
      var token = Guid.NewGuid();

      var user = new User
      {
        Password = "CorrectPassword".GetAsPasswordHash(),
        Token = new Token
        {
          Value = token
        }
      };

      var userStore = Substitute.For<IUserStore>();
      userStore.GetUserAsync(Arg.Any<string>()).Returns(user);

      var tokenStore = Substitute.For<ITokenStore>();
      tokenStore.IsTokenValid(Arg.Any<Token>()).Returns(true);

      var userSettings = CreateUserSettings();
      var log = Substitute.For<ILogger>();

      var testObject = new UserController(userStore, tokenStore, userSettings, log);

      // Act.
      await testObject.Login("username", "CorrectPassword");

      // Assert.
      Assert.AreEqual(token, user.Token.Value);
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public async Task Login_GivenCurrentTokenExpired_ShouldReturnNewToken()
    {
      // Arrange.
      var token = Guid.NewGuid();

      var user = new User
      {
        Password = "CorrectPassword".GetAsPasswordHash(),
        Token = new Token
        {
          Value = token
        }
      };

      var userStore = Substitute.For<IUserStore>();
      userStore.GetUserAsync(Arg.Any<string>()).Returns(user);

      var tokenStore = Substitute.For<ITokenStore>();
      tokenStore.GetNewTokenAsync().Returns(new Token());

      var userSettings = CreateUserSettings();
      var log = Substitute.For<ILogger>();

      var testObject = new UserController(userStore, tokenStore, userSettings, log);

      // Act.
      await testObject.Login("username", "CorrectPassword");

      // Assert.
      Assert.AreNotEqual(token, user.Token.Value);
    }

    //=============================================================================================

    private static IUserSettings CreateUserSettings()
    {
      var settings = Substitute.For<IUserSettings>();

      settings.UserPasswordMinLength.Returns(8);
      
      return settings;
    }

    //---------------------------------------------------------------------------------------------
  }
}
