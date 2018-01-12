using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using Newtonsoft.Json;
using NUnit.Framework;
using NSubstitute;
using BearChaser.Controllers;
using BearChaser.DataTransferObjects;
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
      IHttpActionResult result = await testObject.Register(
        new UserLoginData
        {
          Username = "  ",
          Password = "password"
        });

      var badRequestResult = result as BadRequestErrorMessageResult;

      // Assert.
      Assert.NotNull(badRequestResult);
      Assert.AreEqual("Username can't be blank.", badRequestResult.Message);
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
      IHttpActionResult result = await testObject.Register(
        new UserLoginData
        {
          Username = "username",
          Password = "password"
        });

      var badRequestResult = result as BadRequestErrorMessageResult;

      // Assert.
      Assert.NotNull(badRequestResult);
      Assert.AreEqual("Username already exists.", badRequestResult.Message);
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
      IHttpActionResult result = await testObject.Register(
        new UserLoginData
        {
          Username = "username",
          Password = "1234567"
        });

      var badRequestResult = result as BadRequestErrorMessageResult;

      // Assert.
      Assert.NotNull(badRequestResult);
      Assert.AreEqual("Password must be 8 or more characters long.", badRequestResult.Message);
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
      IHttpActionResult result = await testObject.Register(
        new UserLoginData
        {
          Username = "username",
          Password = "12345678"
        });

      var okResult = result as OkNegotiatedContentResult<string>;

      // Assert.
      Assert.NotNull(okResult);
      Assert.AreEqual("User registered.", okResult.Content);
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
      IHttpActionResult result = await testObject.Login(
        new UserLoginData
        {
          Username = "username",
          Password = "CorrectPassword"
        });

      var jsonResult = result as OkNegotiatedContentResult<string>;
      var returnedToken = JsonConvert.DeserializeObject<Guid>(jsonResult.Content);

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
      IHttpActionResult result = await testObject.Login(
        new UserLoginData
        {
          Username = "username",
          Password = string.Empty
        });

      // Assert.
      var httpNotFoundResult = result as NotFoundResult;

      Assert.NotNull(httpNotFoundResult);
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
      IHttpActionResult result = await testObject.Login(
        new UserLoginData
        {
          Username = "username",
          Password = "IncorrectPassword"
        });

      // Assert.
      var httpStatusCodeResult = result as UnauthorizedResult;

      Assert.NotNull(httpStatusCodeResult);
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
      IHttpActionResult result = await testObject.Login(
        new UserLoginData
        {
          Username = "username",
          Password = "CorrectPassword"
        });

      // Assert.
      Assert.NotNull(user.TokenId);
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
      IHttpActionResult result = await testObject.Login(
        new UserLoginData
        {
          Username = "username",
          Password = "CorrectPassword"
        });

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
      IHttpActionResult result = await testObject.Login(
        new UserLoginData
        {
          Username = "username",
          Password = "CorrectPassword"
        });

      // Assert.
      var jsonResult = result as OkNegotiatedContentResult<string>;
      var returnedToken = JsonConvert.DeserializeObject<Guid>(jsonResult.Content);

      Assert.AreNotEqual(token, returnedToken);
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
