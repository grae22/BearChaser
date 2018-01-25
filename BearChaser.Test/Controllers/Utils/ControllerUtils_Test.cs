using System;
using System.Net.Http;
using System.Security.Authentication;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using NSubstitute;
using NUnit.Framework;
using BearChaser.Controllers.Utils;
using BearChaser.Exceptions;
using BearChaser.Models;
using BearChaser.Stores;
using BearChaser.Utils.Logging;

namespace BearChaser.Test.Controllers.Utils
{
  [TestFixture]
  [Category("ControllerUtils")]
  internal class ControllerUtils_Test
  {
    //---------------------------------------------------------------------------------------------

    [Test]
    public void GetUserForRequestHeaderTokenAsync_GivenNoToken_ShouldRaiseException()
    {
      // Arrange.
      var controller = Substitute.For<ApiController>();
      var tokenStore = Substitute.For<ITokenStore>();
      var userStore = Substitute.For<IUserStore>();
      var log = Substitute.For<ILogger>();

      controller.ControllerContext = new HttpControllerContext
      {
        Request = new HttpRequestMessage()
      };

      // Act & Assert.
      Assert.That(
        async () => await ControllerUtils.GetUserForRequestHeaderTokenAsync(controller, tokenStore, userStore, log),
        Throws.TypeOf<AuthenticationException>().With.Message.EqualTo("No token provided."));
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public void GetUserForRequestHeaderTokenAsync_GivenNonGuidToken_ShouldRaiseException()
    {
      // Arrange.
      var controller = Substitute.For<ApiController>();
      var tokenStore = Substitute.For<ITokenStore>();
      var userStore = Substitute.For<IUserStore>();
      var log = Substitute.For<ILogger>();

      controller.ControllerContext = new HttpControllerContext
      {
        Request = new HttpRequestMessage()
      };
      controller.ControllerContext.Request.Headers.Add("auth", "SomeNonGuidToken");

      // Act & Assert.
      Assert.That(
        async () => await ControllerUtils.GetUserForRequestHeaderTokenAsync(controller, tokenStore, userStore, log),
        Throws.TypeOf<AuthenticationException>().With.Message.EqualTo("Invalid user token format."));
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public void GetUserForRequestHeaderTokenAsync_GivenTokenNotInStore_ShouldRaiseException()
    {
      // Arrange.
      var controller = Substitute.For<ApiController>();
      var tokenStore = Substitute.For<ITokenStore>();
      var userStore = Substitute.For<IUserStore>();
      var log = Substitute.For<ILogger>();

      controller.ControllerContext = new HttpControllerContext
      {
        Request = new HttpRequestMessage()
      };
      controller.ControllerContext.Request.Headers.Add("auth", Guid.NewGuid().ToString());

      // Act & Assert.
      Assert.That(
        async () => await ControllerUtils.GetUserForRequestHeaderTokenAsync(controller, tokenStore, userStore, log),
        Throws.TypeOf<AuthenticationException>().With.Message.EqualTo("User token not found, it may have expired."));
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public void GetUserForRequestHeaderTokenAsync_GivenValidTokenButUserNotFound_ShouldRaiseException()
    {
      // Arrange.
      var controller = Substitute.For<ApiController>();
      var tokenStore = Substitute.For<ITokenStore>();
      var userStore = Substitute.For<IUserStore>();
      var log = Substitute.For<ILogger>();
      var guid = Guid.NewGuid();

      controller.ControllerContext = new HttpControllerContext
      {
        Request = new HttpRequestMessage()
      };
      controller.ControllerContext.Request.Headers.Add("auth", guid.ToString());

      tokenStore.GetExistingValidTokenByGuidAsync(guid).Returns(new Token());

      // Act & Assert.
      Assert.That(
        async () => await ControllerUtils.GetUserForRequestHeaderTokenAsync(controller, tokenStore, userStore, log),
        Throws.TypeOf<InternalServerException>());
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public async Task GetUserForRequestHeaderTokenAsync_GivenValidToken_ShouldReturnUser()
    {
      // Arrange.
      var controller = Substitute.For<ApiController>();
      var tokenStore = Substitute.For<ITokenStore>();
      var userStore = Substitute.For<IUserStore>();
      var log = Substitute.For<ILogger>();
      var guid = Guid.NewGuid();

      controller.ControllerContext = new HttpControllerContext
      {
        Request = new HttpRequestMessage()
      };
      controller.ControllerContext.Request.Headers.Add("auth", guid.ToString());

      tokenStore.GetExistingValidTokenByGuidAsync(guid).Returns(new Token());
      userStore.GetUserAsync(Arg.Any<Token>()).Returns(new User());

      // Act.
      User user = await ControllerUtils.GetUserForRequestHeaderTokenAsync(controller, tokenStore, userStore, log);
      
      // Assert.
      Assert.NotNull(user);
    }

    //---------------------------------------------------------------------------------------------
  }
}
