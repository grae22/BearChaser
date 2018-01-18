using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Results;
using AutoMapper;
using NUnit.Framework;
using NSubstitute;
using Newtonsoft.Json;
using BearChaser.Controllers.Api;
using BearChaser.DataTransferObjects;
using BearChaser.Models;
using BearChaser.Stores;
using BearChaser.Utils.Logging;

namespace BearChaser.Test.Controllers.Api
{
  [TestFixture]
  [Category("GoalController")]
  internal class GoalController_Test
  {
    //---------------------------------------------------------------------------------------------

    [Test]
    public void Constructor_GivenNullGoalStore_ShouldRaiseException()
    {
      // Arrange.
      var userStore = Substitute.For<IUserStore>();
      var tokenStore = Substitute.For<ITokenStore>();
      var log = Substitute.For<ILogger>();

      // Act & Assert.
      Assert.That(
        () => new GoalController(null, userStore, tokenStore, log),
        Throws
          .TypeOf<ArgumentException>()
          .With.Message.EqualTo("GoalStore cannot be null."));
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public void Constructor_GivenNullUserStore_ShouldRaiseException()
    {
      // Arrange.
      var goalStore = Substitute.For<IGoalStore>();
      var tokenStore = Substitute.For<ITokenStore>();
      var log = Substitute.For<ILogger>();

      // Act & Assert.
      Assert.That(
        () => new GoalController(goalStore, null, tokenStore, log),
        Throws
          .TypeOf<ArgumentException>()
          .With.Message.EqualTo("UserStore cannot be null."));
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public void Constructor_GivenNullTokenStore_ShouldRaiseException()
    {
      // Arrange.
      var goalStore = Substitute.For<IGoalStore>();
      var userStore = Substitute.For<IUserStore>();
      var log = Substitute.For<ILogger>();

      // Act & Assert.
      Assert.That(
        () => new GoalController(goalStore, userStore, null, log),
        Throws
          .TypeOf<ArgumentException>()
          .With.Message.EqualTo("TokenStore cannot be null."));
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public void Constructor_GivenNullLogger_ShouldRaiseException()
    {
      // Arrange.
      var goalStore = Substitute.For<IGoalStore>();
      var userStore = Substitute.For<IUserStore>();
      var tokenStore = Substitute.For<ITokenStore>();

      // Act & Assert.
      Assert.That(
        () => new GoalController(goalStore, userStore, tokenStore, null),
        Throws
          .TypeOf<ArgumentException>()
          .With.Message.EqualTo("Logger cannot be null."));
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public async Task GetGoalsAsync_GivenUserToken_ShouldReturnUsersGoals()
    {
      // Arrange.
      MapperConfig.Initialise();

      var goalStore = Substitute.For<IGoalStore>();
      var userStore = Substitute.For<IUserStore>();
      var tokenStore = Substitute.For<ITokenStore>();
      var log = Substitute.For<ILogger>();
      var testObject = new GoalController(goalStore, userStore, tokenStore, log);
      var guid = Guid.NewGuid();
      var userToken = new Token();

      testObject.ControllerContext = new HttpControllerContext
      {
        Request = new HttpRequestMessage()
      };
      testObject.ControllerContext.Request.Headers.Add("auth", guid.ToString());

      tokenStore.GetExistingValidTokenByGuidAsync(guid).Returns(userToken);
      userStore.GetUserAsync(userToken).Returns(new User { Id = 123 });

      var goals = new List<Goal>
      {
        new Goal
        {
          Id = 10,
          UserId = 123,
          Name = "Some Goal",
          Period = Goal.TimePeriod.Day,
          FrequencyWithinPeriod = 1
        }
      };

      goalStore.GetGoalsAsync(123).Returns(goals);

      // Act.
      var result = await testObject.GetGoalsAsync();

      // Assert.
      var okResult = result as OkNegotiatedContentResult<string>;

      var goalDatas = new List<GoalData>
      {
        Mapper.Map<GoalData>(goals[0])
      };

      Assert.NotNull(okResult);
      Assert.AreEqual(JsonConvert.SerializeObject(goalDatas), okResult.Content);
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public async Task GetGoalsAsync_GivenNoUserToken_ShouldReturnBadRequest()
    {
      // Arrange.
      var goalStore = Substitute.For<IGoalStore>();
      var userStore = Substitute.For<IUserStore>();
      var tokenStore = Substitute.For<ITokenStore>();
      var log = Substitute.For<ILogger>();
      var testObject = new GoalController(goalStore, userStore, tokenStore, log);

      // Act.
      var result = await testObject.GetGoalsAsync();

      // Assert.
      var badRequestResult = result as BadRequestErrorMessageResult;

      Assert.NotNull(badRequestResult);
      Assert.AreEqual("No token provided.", badRequestResult.Message);
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public async Task GetGoalsAsync_GivenInvalidUserToken_ShouldReturnBadRequest()
    {
      // Arrange.
      var goalStore = Substitute.For<IGoalStore>();
      var userStore = Substitute.For<IUserStore>();
      var tokenStore = Substitute.For<ITokenStore>();
      var log = Substitute.For<ILogger>();
      var testObject = new GoalController(goalStore, userStore, tokenStore, log);

      testObject.ControllerContext = new HttpControllerContext
      {
        Request = new HttpRequestMessage()
      };
      testObject.ControllerContext.Request.Headers.Add("auth", "Not A Guid");

      // Act.
      var result = await testObject.GetGoalsAsync();

      // Assert.
      var badRequestResult = result as BadRequestErrorMessageResult;

      Assert.NotNull(badRequestResult);
      Assert.AreEqual("Invalid user token format.", badRequestResult.Message);
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public async Task GetGoalsAsync_GivenUserTokenWithNoMatchingToken_ShouldReturnBadRequest()
    {
      // Arrange.
      var goalStore = Substitute.For<IGoalStore>();
      var userStore = Substitute.For<IUserStore>();
      var tokenStore = Substitute.For<ITokenStore>();
      var log = Substitute.For<ILogger>();
      var testObject = new GoalController(goalStore, userStore, tokenStore, log);
      var guid = Guid.NewGuid();

      testObject.ControllerContext = new HttpControllerContext
      {
        Request = new HttpRequestMessage()
      };
      testObject.ControllerContext.Request.Headers.Add("auth", guid.ToString());

      tokenStore.GetExistingValidTokenByGuidAsync(guid).Returns((Token)null);

      // Act.
      var result = await testObject.GetGoalsAsync();

      // Assert.
      var badRequestResult = result as BadRequestErrorMessageResult;

      Assert.NotNull(badRequestResult);
      Assert.AreEqual("User token not found, it may have expired.", badRequestResult.Message);
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public async Task GetGoalsAsync_GivenValidParamsAndUserIsntFound_ShouldReturnServerError()
    {
      // Arrange.
      var goalStore = Substitute.For<IGoalStore>();
      var userStore = Substitute.For<IUserStore>();
      var tokenStore = Substitute.For<ITokenStore>();
      var log = Substitute.For<ILogger>();
      var testObject = new GoalController(goalStore, userStore, tokenStore, log);
      var guid = Guid.NewGuid();
      var userToken = new Token();

      testObject.ControllerContext = new HttpControllerContext
      {
        Request = new HttpRequestMessage()
      };
      testObject.ControllerContext.Request.Headers.Add("auth", guid.ToString());

      tokenStore.GetExistingValidTokenByGuidAsync(guid).Returns(userToken);
      userStore.GetUserAsync(userToken).Returns((User)null);

      // Act.
      var result = await testObject.GetGoalsAsync();

      // Assert.
      var serverErrorResult = result as InternalServerErrorResult;

      Assert.NotNull(serverErrorResult);
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public async Task CreateGoalAsync_GivenValidParams_ShouldAddGoalToStore()
    {
      // Arrange.
      MapperConfig.Initialise();

      var goalStore = Substitute.For<IGoalStore>();
      var userStore = Substitute.For<IUserStore>();
      var tokenStore = Substitute.For<ITokenStore>();
      var log = Substitute.For<ILogger>();
      var testObject = new GoalController(goalStore, userStore, tokenStore, log);
      var guid = Guid.NewGuid();
      var userToken = new Token();

      testObject.ControllerContext = new HttpControllerContext
      {
        Request = new HttpRequestMessage()
      };
      testObject.ControllerContext.Request.Headers.Add("auth", guid.ToString());

      tokenStore.GetExistingValidTokenByGuidAsync(guid).Returns(userToken);
      userStore.GetUserAsync(userToken).Returns(new User { Id = 123 });

      var returnedGoal = new Goal
      {
        Id = 10,
        UserId = 123,
        Name = "NewGoal",
        Period = (Goal.TimePeriod) 1,
        FrequencyWithinPeriod = 2
      };

      var returnedGoalData = Mapper.Map<GoalData>(returnedGoal);

      goalStore.CreateGoalAsync(123, "NewGoal", (Goal.TimePeriod)1, 2).Returns(returnedGoal);

      // Act.
      var result = await testObject.CreateGoalAsync(
        new GoalData
        {
          Name = "NewGoal",
          Period = 1,
          FrequencyWithinPeriod = 2
        });

      // Assert.
      await goalStore.Received(1).CreateGoalAsync(123, "NewGoal", (Goal.TimePeriod)1, 2);

      var okResult = result as OkNegotiatedContentResult<string>;

      Assert.NotNull(okResult);
      Assert.AreEqual(JsonConvert.SerializeObject(returnedGoalData), okResult.Content);
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public async Task CreateGoalAsync_GivenNoUserToken_ShouldReturnBadRequest()
    {
      // Arrange.
      var goalStore = Substitute.For<IGoalStore>();
      var userStore = Substitute.For<IUserStore>();
      var tokenStore = Substitute.For<ITokenStore>();
      var log = Substitute.For<ILogger>();
      var testObject = new GoalController(goalStore, userStore, tokenStore, log);

      // Act.
      var result = await testObject.CreateGoalAsync(
        new GoalData
        {
          Name = "NewGoal",
          Period = 1,
          FrequencyWithinPeriod = 2
        });

      // Assert.
      var badRequestResult = result as BadRequestErrorMessageResult;

      Assert.NotNull(badRequestResult);
      Assert.AreEqual("No token provided.", badRequestResult.Message);
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public async Task CreateGoalAsync_GivenInvalidUserToken_ShouldReturnBadRequest()
    {
      // Arrange.
      var goalStore = Substitute.For<IGoalStore>();
      var userStore = Substitute.For<IUserStore>();
      var tokenStore = Substitute.For<ITokenStore>();
      var log = Substitute.For<ILogger>();
      var testObject = new GoalController(goalStore, userStore, tokenStore, log);

      testObject.ControllerContext = new HttpControllerContext
      {
        Request = new HttpRequestMessage()
      };
      testObject.ControllerContext.Request.Headers.Add("auth", "Not A Guid");

      // Act.
      var result = await testObject.CreateGoalAsync(
        new GoalData
        {
          Name = "NewGoal",
          Period = 1,
          FrequencyWithinPeriod = 2
        });

      // Assert.
      var badRequestResult = result as BadRequestErrorMessageResult;

      Assert.NotNull(badRequestResult);
      Assert.AreEqual("Invalid user token format.", badRequestResult.Message);
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public async Task CreateGoalAsync_GivenUserTokenWithNoMatchingToken_ShouldReturnBadRequest()
    {
      // Arrange.
      var goalStore = Substitute.For<IGoalStore>();
      var userStore = Substitute.For<IUserStore>();
      var tokenStore = Substitute.For<ITokenStore>();
      var log = Substitute.For<ILogger>();
      var testObject = new GoalController(goalStore, userStore, tokenStore, log);
      var guid = Guid.NewGuid();

      testObject.ControllerContext = new HttpControllerContext
      {
        Request = new HttpRequestMessage()
      };
      testObject.ControllerContext.Request.Headers.Add("auth", guid.ToString());

      tokenStore.GetExistingValidTokenByGuidAsync(guid).Returns((Token)null);

      // Act.
      var result = await testObject.CreateGoalAsync(
        new GoalData
        {
          Name = "NewGoal",
          Period = 1,
          FrequencyWithinPeriod = 2
        });

      // Assert.
      var badRequestResult = result as BadRequestErrorMessageResult;

      Assert.NotNull(badRequestResult);
      Assert.AreEqual("User token not found, it may have expired.", badRequestResult.Message);
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public async Task CreateGoalAsync_GivenValidParamsAndUserIsntFound_ShouldReturnServerError()
    {
      // Arrange.
      var goalStore = Substitute.For<IGoalStore>();
      var userStore = Substitute.For<IUserStore>();
      var tokenStore = Substitute.For<ITokenStore>();
      var log = Substitute.For<ILogger>();
      var testObject = new GoalController(goalStore, userStore, tokenStore, log);
      var guid = Guid.NewGuid();
      var userToken = new Token();

      testObject.ControllerContext = new HttpControllerContext
      {
        Request = new HttpRequestMessage()
      };
      testObject.ControllerContext.Request.Headers.Add("auth", guid.ToString());

      tokenStore.GetExistingValidTokenByGuidAsync(guid).Returns(userToken);
      userStore.GetUserAsync(userToken).Returns((User)null);

      // Act.
      var result = await testObject.CreateGoalAsync(
        new GoalData
        {
          Name = "NewGoal",
          Period = 1,
          FrequencyWithinPeriod = 2
        });

      // Assert.
      var serverErrorResult = result as InternalServerErrorResult;

      Assert.NotNull(serverErrorResult);
    }

    //---------------------------------------------------------------------------------------------
  }
}
