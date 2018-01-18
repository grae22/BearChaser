using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Results;
using AutoMapper;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using BearChaser.Controllers.Api;
using BearChaser.DataTransferObjects;
using BearChaser.Models;
using BearChaser.Stores;
using BearChaser.Utils.Logging;

namespace BearChaser.Test.Controllers.Api
{
  [TestFixture]
  [Category("GoalAttemptController")]
  internal class GoalAttemptController_Test
  {
    //---------------------------------------------------------------------------------------------

    [Test]
    public void Constructor_GivenNullAttemptStore_ShouldRaiseException()
    {
      // Arrange.
      var goalStore = Substitute.For<IGoalStore>();
      var tokenStore = Substitute.For<ITokenStore>();
      var userStore = Substitute.For<IUserStore>();
      var log = Substitute.For<ILogger>();

      // Act & Assert.
      Assert.That(
        () => new GoalAttemptController(null, goalStore, tokenStore, userStore, log),
        Throws.ArgumentNullException);
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public void Constructor_GivenNullGoalStore_ShouldRaiseException()
    {
      // Arrange.
      var attemptStore = Substitute.For<IGoalAttemptStore>();
      var tokenStore = Substitute.For<ITokenStore>();
      var userStore = Substitute.For<IUserStore>();
      var log = Substitute.For<ILogger>();

      // Act & Assert.
      Assert.That(
        () => new GoalAttemptController(attemptStore, null, tokenStore, userStore, log),
        Throws.ArgumentNullException);
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public void Constructor_GivenNullTokenStore_ShouldRaiseException()
    {
      // Arrange.
      var attemptStore = Substitute.For<IGoalAttemptStore>();
      var goalStore = Substitute.For<IGoalStore>();
      var userStore = Substitute.For<IUserStore>();
      var log = Substitute.For<ILogger>();

      // Act & Assert.
      Assert.That(
        () => new GoalAttemptController(attemptStore, goalStore, null, userStore, log),
        Throws.ArgumentNullException);
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public void Constructor_GivenNullUserStore_ShouldRaiseException()
    {
      // Arrange.
      var attemptStore = Substitute.For<IGoalAttemptStore>();
      var goalStore = Substitute.For<IGoalStore>();
      var tokenStore = Substitute.For<ITokenStore>();
      var log = Substitute.For<ILogger>();

      // Act & Assert.
      Assert.That(
        () => new GoalAttemptController(attemptStore, goalStore, tokenStore, null, log),
        Throws.ArgumentNullException);
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public void Constructor_GivenNullLogger_ShouldRaiseException()
    {
      // Arrange.
      var attemptStore = Substitute.For<IGoalAttemptStore>();
      var goalStore = Substitute.For<IGoalStore>();
      var tokenStore = Substitute.For<ITokenStore>();
      var userStore = Substitute.For<IUserStore>();

      // Act & Assert.
      Assert.That(
        () => new GoalAttemptController(attemptStore, goalStore, tokenStore, userStore, null),
        Throws.ArgumentNullException);
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public async Task CreateAttemptAsync_GivenValidAttempt_ShouldAddToDb()
    {
      // Arrange.
      var attemptStore = Substitute.For<IGoalAttemptStore>();
      var goalStore = Substitute.For<IGoalStore>();
      var tokenStore = Substitute.For<ITokenStore>();
      var userStore = Substitute.For<IUserStore>();
      var log = Substitute.For<ILogger>();
      var testObject = new GoalAttemptController(attemptStore, goalStore, tokenStore, userStore, log);
      var user = new User { Id = 1 };
      var goal = new Goal { UserId = 1 };

      testObject.ControllerContext = new HttpControllerContext
      {
        Request = new HttpRequestMessage()
      };
      testObject.ControllerContext.Request.Headers.Add("auth", Guid.NewGuid().ToString());

      tokenStore.GetExistingValidTokenByGuidAsync(Arg.Any<Guid>()).Returns(new Token());
      userStore.GetUserAsync(Arg.Any<Token>()).Returns(user);
      goalStore.GetGoalAsync(123).Returns(goal);

      var attempt = new GoalAttemptData
      {
        GoalId = 123
      };

      // Act.
      var result = await testObject.CreateAttemptAsync(attempt);

      // Assert.
      var statusResult = result as StatusCodeResult;

      Assert.NotNull(statusResult);
      Assert.AreEqual(HttpStatusCode.Created, statusResult.StatusCode);

      await attemptStore.Received(1).CreateAttemptAsync(123);
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public async Task CreateAttemptAsync_GivenNoAuthToken_ShouldReturnBadRequest()
    {
      // Arrange.
      var attemptStore = Substitute.For<IGoalAttemptStore>();
      var goalStore = Substitute.For<IGoalStore>();
      var tokenStore = Substitute.For<ITokenStore>();
      var userStore = Substitute.For<IUserStore>();
      var log = Substitute.For<ILogger>();
      var testObject = new GoalAttemptController(attemptStore, goalStore, tokenStore, userStore, log);

      var attempt = new GoalAttemptData
      {
        GoalId = 123
      };

      // Act.
      var result = await testObject.CreateAttemptAsync(attempt);

      // Assert.
      var badRequestResult = result as BadRequestErrorMessageResult;

      Assert.NotNull(badRequestResult);
      Assert.AreEqual("No token provided.", badRequestResult.Message);
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public async Task CreateAttemptAsync_GivenValidTokenButNoUser_ShouldReturnInternalServerError()
    {
      // Arrange.
      var attemptStore = Substitute.For<IGoalAttemptStore>();
      var goalStore = Substitute.For<IGoalStore>();
      var tokenStore = Substitute.For<ITokenStore>();
      var userStore = Substitute.For<IUserStore>();
      var log = Substitute.For<ILogger>();
      var testObject = new GoalAttemptController(attemptStore, goalStore, tokenStore, userStore, log);

      testObject.ControllerContext = new HttpControllerContext
      {
        Request = new HttpRequestMessage()
      };
      testObject.ControllerContext.Request.Headers.Add("auth", Guid.NewGuid().ToString());

      tokenStore.GetExistingValidTokenByGuidAsync(Arg.Any<Guid>()).Returns(new Token());

      var attempt = new GoalAttemptData
      {
        GoalId = 123
      };

      // Act.
      var result = await testObject.CreateAttemptAsync(attempt);

      // Assert.
      var serverErrorResult = result as InternalServerErrorResult;

      Assert.NotNull(serverErrorResult);
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public async Task CreateAttemptAsync_GivenUnknownGoalId_ShouldReturnNotFound()
    {
      // Arrange.
      var attemptStore = Substitute.For<IGoalAttemptStore>();
      var goalStore = Substitute.For<IGoalStore>();
      var tokenStore = Substitute.For<ITokenStore>();
      var userStore = Substitute.For<IUserStore>();
      var log = Substitute.For<ILogger>();
      var testObject = new GoalAttemptController(attemptStore, goalStore, tokenStore, userStore, log);

      testObject.ControllerContext = new HttpControllerContext
      {
        Request = new HttpRequestMessage()
      };
      testObject.ControllerContext.Request.Headers.Add("auth", Guid.NewGuid().ToString());

      tokenStore.GetExistingValidTokenByGuidAsync(Arg.Any<Guid>()).Returns(new Token());
      userStore.GetUserAsync(Arg.Any<Token>()).Returns(new User());

      var attempt = new GoalAttemptData
      {
        GoalId = 123
      };

      // Act.
      var result = await testObject.CreateAttemptAsync(attempt);

      // Assert.
      var notFoundResult = result as NotFoundResult;

      Assert.NotNull(notFoundResult);
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public async Task CreateAttemptAsync_GivenAnotherUsersGoal_ShouldReturnBadRequest()
    {
      // Arrange.
      var attemptStore = Substitute.For<IGoalAttemptStore>();
      var goalStore = Substitute.For<IGoalStore>();
      var tokenStore = Substitute.For<ITokenStore>();
      var userStore = Substitute.For<IUserStore>();
      var log = Substitute.For<ILogger>();
      var testObject = new GoalAttemptController(attemptStore, goalStore, tokenStore, userStore, log);
      var currentUser = new User { Id = 1 };
      var otherUsersGoal = new Goal { UserId = 100 };

      testObject.ControllerContext = new HttpControllerContext
      {
        Request = new HttpRequestMessage()
      };
      testObject.ControllerContext.Request.Headers.Add("auth", Guid.NewGuid().ToString());

      tokenStore.GetExistingValidTokenByGuidAsync(Arg.Any<Guid>()).Returns(new Token());
      userStore.GetUserAsync(Arg.Any<Token>()).Returns(currentUser);
      goalStore.GetGoalAsync(123).Returns(otherUsersGoal);

      var attempt = new GoalAttemptData
      {
        GoalId = 123
      };

      // Act.
      var result = await testObject.CreateAttemptAsync(attempt);

      // Assert.
      var notFoundResult = result as NotFoundResult;

      Assert.NotNull(notFoundResult);
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public async Task DeleteAttemptAsync_GivenValidAttempt_ShouldRemoveFromDb()
    {
      // Arrange.
      var attemptStore = Substitute.For<IGoalAttemptStore>();
      var goalStore = Substitute.For<IGoalStore>();
      var tokenStore = Substitute.For<ITokenStore>();
      var userStore = Substitute.For<IUserStore>();
      var log = Substitute.For<ILogger>();
      var testObject = new GoalAttemptController(attemptStore, goalStore, tokenStore, userStore, log);
      var user = new User { Id = 1 };
      var goal = new Goal { UserId = 1 };

      testObject.ControllerContext = new HttpControllerContext
      {
        Request = new HttpRequestMessage()
      };
      testObject.ControllerContext.Request.Headers.Add("auth", Guid.NewGuid().ToString());

      tokenStore.GetExistingValidTokenByGuidAsync(Arg.Any<Guid>()).Returns(new Token());
      userStore.GetUserAsync(Arg.Any<Token>()).Returns(user);
      goalStore.GetGoalAsync(Arg.Any<int>()).Returns(goal);
      attemptStore.GetAttemptAsync(101).Returns(new GoalAttempt());

      // Act.
      var result = await testObject.DeleteAttemptAsync(101);

      // Assert.
      var okResult = result as OkResult;

      Assert.NotNull(okResult);

      await attemptStore.Received(1).RemoveAttemptAsync(101);
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public async Task DeleteAttemptAsync_GivenNoAuthToken_ShouldReturnBadRequest()
    {
      // Arrange.
      var attemptStore = Substitute.For<IGoalAttemptStore>();
      var goalStore = Substitute.For<IGoalStore>();
      var tokenStore = Substitute.For<ITokenStore>();
      var userStore = Substitute.For<IUserStore>();
      var log = Substitute.For<ILogger>();
      var testObject = new GoalAttemptController(attemptStore, goalStore, tokenStore, userStore, log);

      // Act.
      var result = await testObject.DeleteAttemptAsync(123);

      // Assert.
      var badRequestResult = result as BadRequestErrorMessageResult;

      Assert.NotNull(badRequestResult);
      Assert.AreEqual("No token provided.", badRequestResult.Message);
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public async Task DeleteAttemptAsync_GivenValidTokenButNoUser_ShouldReturnInternalServerError()
    {
      // Arrange.
      var attemptStore = Substitute.For<IGoalAttemptStore>();
      var goalStore = Substitute.For<IGoalStore>();
      var tokenStore = Substitute.For<ITokenStore>();
      var userStore = Substitute.For<IUserStore>();
      var log = Substitute.For<ILogger>();
      var testObject = new GoalAttemptController(attemptStore, goalStore, tokenStore, userStore, log);

      testObject.ControllerContext = new HttpControllerContext
      {
        Request = new HttpRequestMessage()
      };
      testObject.ControllerContext.Request.Headers.Add("auth", Guid.NewGuid().ToString());

      tokenStore.GetExistingValidTokenByGuidAsync(Arg.Any<Guid>()).Returns(new Token());

      // Act.
      var result = await testObject.DeleteAttemptAsync(123);

      // Assert.
      var serverErrorResult = result as InternalServerErrorResult;

      Assert.NotNull(serverErrorResult);
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public async Task DeleteAttemptAsync_GivenUnknownGoalId_ShouldReturnNotFound()
    {
      // Arrange.
      var attemptStore = Substitute.For<IGoalAttemptStore>();
      var goalStore = Substitute.For<IGoalStore>();
      var tokenStore = Substitute.For<ITokenStore>();
      var userStore = Substitute.For<IUserStore>();
      var log = Substitute.For<ILogger>();
      var testObject = new GoalAttemptController(attemptStore, goalStore, tokenStore, userStore, log);

      testObject.ControllerContext = new HttpControllerContext
      {
        Request = new HttpRequestMessage()
      };
      testObject.ControllerContext.Request.Headers.Add("auth", Guid.NewGuid().ToString());

      tokenStore.GetExistingValidTokenByGuidAsync(Arg.Any<Guid>()).Returns(new Token());
      userStore.GetUserAsync(Arg.Any<Token>()).Returns(new User());
      attemptStore.GetAttemptAsync(123).Returns(new GoalAttempt());

      // Act.
      var result = await testObject.DeleteAttemptAsync(123);

      // Assert.
      var notFoundResult = result as NotFoundResult;

      Assert.NotNull(notFoundResult);
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public async Task DeleteAttemptAsync_GivenAnotherUsersGoal_ShouldReturnBadRequest()
    {
      // Arrange.
      var attemptStore = Substitute.For<IGoalAttemptStore>();
      var goalStore = Substitute.For<IGoalStore>();
      var tokenStore = Substitute.For<ITokenStore>();
      var userStore = Substitute.For<IUserStore>();
      var log = Substitute.For<ILogger>();
      var testObject = new GoalAttemptController(attemptStore, goalStore, tokenStore, userStore, log);
      var currentUser = new User { Id = 1 };
      var otherUsersGoal = new Goal { UserId = 100 };

      testObject.ControllerContext = new HttpControllerContext
      {
        Request = new HttpRequestMessage()
      };
      testObject.ControllerContext.Request.Headers.Add("auth", Guid.NewGuid().ToString());

      tokenStore.GetExistingValidTokenByGuidAsync(Arg.Any<Guid>()).Returns(new Token());
      userStore.GetUserAsync(Arg.Any<Token>()).Returns(currentUser);
      goalStore.GetGoalAsync(123).Returns(otherUsersGoal);

      // Act.
      var result = await testObject.DeleteAttemptAsync(123);

      // Assert.
      var notFoundResult = result as NotFoundResult;

      Assert.NotNull(notFoundResult);
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public async Task GetAttemptsAsync_GivenValidGoal_ShouldReturnAttempts()
    {
      // Arrange.
      MapperConfig.Initialise();

      var attemptStore = Substitute.For<IGoalAttemptStore>();
      var goalStore = Substitute.For<IGoalStore>();
      var tokenStore = Substitute.For<ITokenStore>();
      var userStore = Substitute.For<IUserStore>();
      var log = Substitute.For<ILogger>();
      var testObject = new GoalAttemptController(attemptStore, goalStore, tokenStore, userStore, log);

      testObject.ControllerContext = new HttpControllerContext
      {
        Request = new HttpRequestMessage()
      };
      testObject.ControllerContext.Request.Headers.Add("auth", Guid.NewGuid().ToString());

      tokenStore.GetExistingValidTokenByGuidAsync(Arg.Any<Guid>()).Returns(new Token());
      userStore.GetUserAsync(Arg.Any<Token>()).Returns(new User());
      goalStore.GetGoalAsync(123).Returns(new Goal());

      var attempts = new List<GoalAttempt>
      {
        new GoalAttempt
        {
          Id = 1,
          GoalId = 123,
          Timestamp = DateTime.Now
        },
        new GoalAttempt
        {
          Id = 2,
          GoalId = 123,
          Timestamp = DateTime.Now
        }
      };

      attemptStore.GetAttemptsAsync(123).Returns(attempts);

      // Act.
      var result = await testObject.GetAttemptsAsync(123);

      // Assert.
      var okResult = result as OkNegotiatedContentResult<string>;

      Assert.NotNull(okResult);

      var attemptDatas = new List<GoalAttemptData>();

      attempts.ForEach(a => attemptDatas.Add(Mapper.Map<GoalAttemptData>(a)));

      Assert.AreEqual(JsonConvert.SerializeObject(attemptDatas), okResult.Content);
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public async Task GetAttemptsAsync_GivenInvalidGoalId_ShouldReturnNotFound()
    {
      // Arrange.
      var attemptStore = Substitute.For<IGoalAttemptStore>();
      var goalStore = Substitute.For<IGoalStore>();
      var tokenStore = Substitute.For<ITokenStore>();
      var userStore = Substitute.For<IUserStore>();
      var log = Substitute.For<ILogger>();
      var testObject = new GoalAttemptController(attemptStore, goalStore, tokenStore, userStore, log);

      testObject.ControllerContext = new HttpControllerContext
      {
        Request = new HttpRequestMessage()
      };
      testObject.ControllerContext.Request.Headers.Add("auth", Guid.NewGuid().ToString());

      tokenStore.GetExistingValidTokenByGuidAsync(Arg.Any<Guid>()).Returns(new Token());
      userStore.GetUserAsync(Arg.Any<Token>()).Returns(new User());
      
      // Act.
      var result = await testObject.GetAttemptsAsync(123);

      // Assert.
      Assert.IsInstanceOf<NotFoundResult>(result);
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public async Task GetAttemptsAsync_GivenNoHeaderToken_ShouldReturnBadRequest()
    {
      // Arrange.
      var attemptStore = Substitute.For<IGoalAttemptStore>();
      var goalStore = Substitute.For<IGoalStore>();
      var tokenStore = Substitute.For<ITokenStore>();
      var userStore = Substitute.For<IUserStore>();
      var log = Substitute.For<ILogger>();
      var testObject = new GoalAttemptController(attemptStore, goalStore, tokenStore, userStore, log);

      // Act.
      var result = await testObject.GetAttemptsAsync(123);

      // Assert.
      Assert.IsInstanceOf<BadRequestErrorMessageResult>(result);
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public async Task GetAttemptsAsync_GivenValidTokenButUserNotFound_ShouldReturnServerError()
    {
      // Arrange.
      var attemptStore = Substitute.For<IGoalAttemptStore>();
      var goalStore = Substitute.For<IGoalStore>();
      var tokenStore = Substitute.For<ITokenStore>();
      var userStore = Substitute.For<IUserStore>();
      var log = Substitute.For<ILogger>();
      var testObject = new GoalAttemptController(attemptStore, goalStore, tokenStore, userStore, log);

      testObject.ControllerContext = new HttpControllerContext
      {
        Request = new HttpRequestMessage()
      };
      testObject.ControllerContext.Request.Headers.Add("auth", Guid.NewGuid().ToString());

      tokenStore.GetExistingValidTokenByGuidAsync(Arg.Any<Guid>()).Returns(new Token());

      // Act.
      var result = await testObject.GetAttemptsAsync(123);

      // Assert.
      Assert.IsInstanceOf<InternalServerErrorResult>(result);
    }

    //---------------------------------------------------------------------------------------------
  }
}
