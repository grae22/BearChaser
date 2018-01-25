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
using BearChaser.Utils;
using BearChaser.Utils.Logging;

namespace BearChaser.Test.Controllers.Api
{
  [TestFixture]
  [Category("GoalController")]
  internal class GoalController_Test
  {
    //---------------------------------------------------------------------------------------------

    private struct TestObjects
    {
      public IGoalStore Goals { get; set; }
      public IGoalAttemptStore Attempts { get; set; }
      public IUserStore Users { get; set; }
      public ITokenStore Tokens { get; set; }
      public IDateTimeSource DateTime { get; set; }
      public ILogger Log { get; set; }
      public GoalController TestObject { get; set; }
    }

    //=============================================================================================

    [Test]
    public void Constructor_GivenNullGoalStore_ShouldRaiseException()
    {
      // Arrange.
      var goalAttemptStore = Substitute.For<IGoalAttemptStore>();
      var userStore = Substitute.For<IUserStore>();
      var tokenStore = Substitute.For<ITokenStore>();
      var dateTime = Substitute.For<IDateTimeSource>();
      var log = Substitute.For<ILogger>();

      // Act & Assert.
      Assert.That(
        () => new GoalController(null, goalAttemptStore, userStore, tokenStore, dateTime, log),
        Throws
          .TypeOf<ArgumentException>()
          .With.Message.EqualTo("GoalStore cannot be null."));
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public void Constructor_GivenNullGoalAttemptStore_ShouldRaiseException()
    {
      // Arrange.
      var goalStore = Substitute.For<IGoalStore>();
      var userStore = Substitute.For<IUserStore>();
      var tokenStore = Substitute.For<ITokenStore>();
      var dateTime = Substitute.For<IDateTimeSource>();
      var log = Substitute.For<ILogger>();

      // Act & Assert.
      Assert.That(
        () => new GoalController(goalStore, null, userStore, tokenStore, dateTime, log),
        Throws
          .TypeOf<ArgumentException>()
          .With.Message.EqualTo("GoalAttemptStore cannot be null."));
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public void Constructor_GivenNullUserStore_ShouldRaiseException()
    {
      // Arrange.
      var goalStore = Substitute.For<IGoalStore>();
      var goalAttemptStore = Substitute.For<IGoalAttemptStore>();
      var tokenStore = Substitute.For<ITokenStore>();
      var dateTime = Substitute.For<IDateTimeSource>();
      var log = Substitute.For<ILogger>();

      // Act & Assert.
      Assert.That(
        () => new GoalController(goalStore, goalAttemptStore, null, tokenStore, dateTime, log),
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
      var goalAttemptStore = Substitute.For<IGoalAttemptStore>();
      var userStore = Substitute.For<IUserStore>();
      var dateTime = Substitute.For<IDateTimeSource>();
      var log = Substitute.For<ILogger>();

      // Act & Assert.
      Assert.That(
        () => new GoalController(goalStore, goalAttemptStore, userStore, null, dateTime, log),
        Throws
          .TypeOf<ArgumentException>()
          .With.Message.EqualTo("TokenStore cannot be null."));
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public void Constructor_GivenNullDateTimeSource_ShouldRaiseException()
    {
      // Arrange.
      var goalStore = Substitute.For<IGoalStore>();
      var goalAttemptStore = Substitute.For<IGoalAttemptStore>();
      var userStore = Substitute.For<IUserStore>();
      var tokenStore = Substitute.For<ITokenStore>();
      var log = Substitute.For<ILogger>();

      // Act & Assert.
      Assert.That(
        () => new GoalController(goalStore, goalAttemptStore, userStore, tokenStore, null, log),
        Throws
          .TypeOf<ArgumentException>()
          .With.Message.EqualTo("DateTimeSource cannot be null."));
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public void Constructor_GivenNullLogger_ShouldRaiseException()
    {
      // Arrange.
      var goalStore = Substitute.For<IGoalStore>();
      var goalAttemptStore = Substitute.For<IGoalAttemptStore>();
      var userStore = Substitute.For<IUserStore>();
      var tokenStore = Substitute.For<ITokenStore>();
      var dateTime = Substitute.For<IDateTimeSource>();

      // Act & Assert.
      Assert.That(
        () => new GoalController(goalStore, goalAttemptStore, userStore, tokenStore, dateTime, null),
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

      var objects = CreateTestObjects();
      var guid = Guid.NewGuid();
      var userToken = new Token();

      objects.TestObject.ControllerContext = new HttpControllerContext
      {
        Request = new HttpRequestMessage()
      };
      objects.TestObject.ControllerContext.Request.Headers.Add("auth", guid.ToString());

      objects.Tokens.GetExistingValidTokenByGuidAsync(guid).Returns(userToken);
      objects.Users.GetUserAsync(userToken).Returns(new User { Id = 123 });

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

      objects.Goals.GetGoalsAsync(123).Returns(goals);

      // Act.
      var result = await objects.TestObject.GetGoalsAsync();

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
      var objects = CreateTestObjects();

      objects.TestObject.ControllerContext = new HttpControllerContext
      {
        Request = new HttpRequestMessage()
      };

      // Act.
      var result = await objects.TestObject.GetGoalsAsync();

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
      var objects = CreateTestObjects();

      objects.TestObject.ControllerContext = new HttpControllerContext
      {
        Request = new HttpRequestMessage()
      };
      objects.TestObject.ControllerContext.Request.Headers.Add("auth", "Not A Guid");

      // Act.
      var result = await objects.TestObject.GetGoalsAsync();

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
      var objects = CreateTestObjects();
      var guid = Guid.NewGuid();

      objects.TestObject.ControllerContext = new HttpControllerContext
      {
        Request = new HttpRequestMessage()
      };
      objects.TestObject.ControllerContext.Request.Headers.Add("auth", guid.ToString());

      objects.Tokens.GetExistingValidTokenByGuidAsync(guid).Returns((Token)null);

      // Act.
      var result = await objects.TestObject.GetGoalsAsync();

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
      var objects = CreateTestObjects();
      var guid = Guid.NewGuid();
      var userToken = new Token();

      objects.TestObject.ControllerContext = new HttpControllerContext
      {
        Request = new HttpRequestMessage()
      };
      objects.TestObject.ControllerContext.Request.Headers.Add("auth", guid.ToString());

      objects.Tokens.GetExistingValidTokenByGuidAsync(guid).Returns(userToken);
      objects.Users.GetUserAsync(userToken).Returns((User)null);

      // Act.
      var result = await objects.TestObject.GetGoalsAsync();

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

      var objects = CreateTestObjects();
      var guid = Guid.NewGuid();
      var userToken = new Token();

      objects.TestObject.ControllerContext = new HttpControllerContext
      {
        Request = new HttpRequestMessage()
      };
      objects.TestObject.ControllerContext.Request.Headers.Add("auth", guid.ToString());

      objects.Tokens.GetExistingValidTokenByGuidAsync(guid).Returns(userToken);
      objects.Users.GetUserAsync(userToken).Returns(new User { Id = 123 });

      var returnedGoal = new Goal
      {
        Id = 10,
        UserId = 123,
        Name = "NewGoal",
        Period = (Goal.TimePeriod) 1,
        FrequencyWithinPeriod = 2,
        StartDate = DateTime.Now
      };

      var returnedGoalData = Mapper.Map<GoalData>(returnedGoal);

      objects.Goals.CreateGoalAsync(123, "NewGoal", (Goal.TimePeriod)1, 2).Returns(returnedGoal);

      // Act.
      var result = await objects.TestObject.CreateGoalAsync(
        new GoalData
        {
          Name = "NewGoal",
          Period = 1,
          FrequencyWithinPeriod = 2,
          StartDate = returnedGoal.StartDate
        });

      // Assert.
      await objects.Goals.Received(1).CreateGoalAsync(123, "NewGoal", (Goal.TimePeriod)1, 2);

      var okResult = result as OkNegotiatedContentResult<string>;

      Assert.NotNull(okResult);
      Assert.AreEqual(JsonConvert.SerializeObject(returnedGoalData), okResult.Content);
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public async Task CreateGoalAsync_GivenNoUserToken_ShouldReturnBadRequest()
    {
      // Arrange.
      var objects = CreateTestObjects();

      objects.TestObject.ControllerContext = new HttpControllerContext
      {
        Request = new HttpRequestMessage()
      };

      // Act.
      var result = await objects.TestObject.CreateGoalAsync(
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
      var objects = CreateTestObjects();

      objects.TestObject.ControllerContext = new HttpControllerContext
      {
        Request = new HttpRequestMessage()
      };
      objects.TestObject.ControllerContext.Request.Headers.Add("auth", "Not A Guid");

      // Act.
      var result = await objects.TestObject.CreateGoalAsync(
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
      var objects = CreateTestObjects();
      var guid = Guid.NewGuid();

      objects.TestObject.ControllerContext = new HttpControllerContext
      {
        Request = new HttpRequestMessage()
      };
      objects.TestObject.ControllerContext.Request.Headers.Add("auth", guid.ToString());

      objects.Tokens.GetExistingValidTokenByGuidAsync(guid).Returns((Token)null);

      // Act.
      var result = await objects.TestObject.CreateGoalAsync(
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
      var objects = CreateTestObjects();
      var guid = Guid.NewGuid();
      var userToken = new Token();

      objects.TestObject.ControllerContext = new HttpControllerContext
      {
        Request = new HttpRequestMessage()
      };
      objects.TestObject.ControllerContext.Request.Headers.Add("auth", guid.ToString());

      objects.Tokens.GetExistingValidTokenByGuidAsync(guid).Returns(userToken);
      objects.Users.GetUserAsync(userToken).Returns((User)null);

      // Act.
      var result = await objects.TestObject.CreateGoalAsync(
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
    
    [Test]
    public async Task GetPeriodStatsAsync_GivenNoUserToken_ShouldReturnBadRequest()
    {
      // Arrange.
      var objects = CreateTestObjects();

      objects.TestObject.ControllerContext = new HttpControllerContext
      {
        Request = new HttpRequestMessage()
      };

      // Act.
      var result = await objects.TestObject.GetPeriodStatsAsync(0);

      // Assert.
      var badRequestResult = result as BadRequestErrorMessageResult;

      Assert.NotNull(badRequestResult);
      Assert.AreEqual("No token provided.", badRequestResult.Message);
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public async Task GetPeriodStatsAsync_GivenInvalidUserToken_ShouldReturnBadRequest()
    {
      // Arrange.
      var objects = CreateTestObjects();

      objects.TestObject.ControllerContext = new HttpControllerContext
      {
        Request = new HttpRequestMessage()
      };
      objects.TestObject.ControllerContext.Request.Headers.Add("auth", "Not A Guid");

      // Act.
      var result = await objects.TestObject.GetPeriodStatsAsync(0);

      // Assert.
      var badRequestResult = result as BadRequestErrorMessageResult;

      Assert.NotNull(badRequestResult);
      Assert.AreEqual("Invalid user token format.", badRequestResult.Message);
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public async Task GetPeriodStatsAsync_GivenUserTokenWithNoMatchingToken_ShouldReturnBadRequest()
    {
      // Arrange.
      var objects = CreateTestObjects();
      var guid = Guid.NewGuid();

      objects.TestObject.ControllerContext = new HttpControllerContext
      {
        Request = new HttpRequestMessage()
      };
      objects.TestObject.ControllerContext.Request.Headers.Add("auth", guid.ToString());

      objects.Tokens.GetExistingValidTokenByGuidAsync(guid).Returns((Token)null);

      // Act.
      var result = await objects.TestObject.GetPeriodStatsAsync(0);

      // Assert.
      var badRequestResult = result as BadRequestErrorMessageResult;

      Assert.NotNull(badRequestResult);
      Assert.AreEqual("User token not found, it may have expired.", badRequestResult.Message);
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public async Task GetPeriodStatsAsync_GivenValidParamsAndUserIsntFound_ShouldReturnServerError()
    {
      // Arrange.
      var objects = CreateTestObjects();
      var guid = Guid.NewGuid();
      var userToken = new Token();

      objects.TestObject.ControllerContext = new HttpControllerContext
      {
        Request = new HttpRequestMessage()
      };
      objects.TestObject.ControllerContext.Request.Headers.Add("auth", guid.ToString());

      objects.Tokens.GetExistingValidTokenByGuidAsync(guid).Returns(userToken);
      objects.Users.GetUserAsync(userToken).Returns((User)null);

      // Act.
      var result = await objects.TestObject.GetPeriodStatsAsync(0);

      // Assert.
      var serverErrorResult = result as InternalServerErrorResult;

      Assert.NotNull(serverErrorResult);
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public async Task GetPeriodStatsAsync_GivenInvalidGoalId_ShouldReturnNotFound()
    {
      // Arrange.
      var objects = CreateTestObjects();
      var guid = Guid.NewGuid();
      var userToken = new Token();

      objects.TestObject.ControllerContext = new HttpControllerContext
      {
        Request = new HttpRequestMessage()
      };
      objects.TestObject.ControllerContext.Request.Headers.Add("auth", guid.ToString());

      objects.Tokens.GetExistingValidTokenByGuidAsync(guid).Returns(userToken);
      objects.Users.GetUserAsync(userToken).Returns(new User());

      // Act.
      var result = await objects.TestObject.GetPeriodStatsAsync(123);

      // Assert.
      Assert.IsInstanceOf<NotFoundResult>(result);
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public async Task GetPeriodStatsAsync_GivenAnotherUsersGoalId_ShouldReturnNotFound()
    {
      // Arrange.
      var objects = CreateTestObjects();
      var guid = Guid.NewGuid();
      var userToken = new Token();

      objects.TestObject.ControllerContext = new HttpControllerContext
      {
        Request = new HttpRequestMessage()
      };
      objects.TestObject.ControllerContext.Request.Headers.Add("auth", guid.ToString());

      objects.Tokens.GetExistingValidTokenByGuidAsync(guid).Returns(userToken);
      objects.Users.GetUserAsync(userToken).Returns(new User { Id = 1 });
      objects.Goals.GetGoalAsync(123).Returns(new Goal { UserId = 666 });

      // Act.
      var result = await objects.TestObject.GetPeriodStatsAsync(123);

      // Assert.
      Assert.IsInstanceOf<NotFoundResult>(result);
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public async Task GetPeriodStatsAsync_GivenGoalIdAndDate_ShouldReturnStatsForGoal()
    {
      // Arrange.
      var objects = CreateTestObjects();
      var guid = Guid.NewGuid();
      var userToken = new Token();
      var now = DateTime.Now;

      objects.TestObject.ControllerContext = new HttpControllerContext
      {
        Request = new HttpRequestMessage()
      };
      objects.TestObject.ControllerContext.Request.Headers.Add("auth", guid.ToString());

      objects.Tokens.GetExistingValidTokenByGuidAsync(guid).Returns(userToken);
      objects.Users.GetUserAsync(userToken).Returns(new User { Id = 100 });
      objects.DateTime.Now.Returns(now);

      objects.Goals.GetGoalAsync(123).Returns(new Goal
      {
        Id = 123,
        UserId = 100,
        Period = Goal.TimePeriod.Day,
        FrequencyWithinPeriod = 2,
        StartDate = now
      });

      objects.Attempts.GetAttemptsAsync(123).Returns(new List<GoalAttempt>
      {
        new GoalAttempt
        {
          Timestamp = now.AddDays(-1)
        },
        new GoalAttempt
        {
          Timestamp = now
        }
      });

      // Act.
      var result = await objects.TestObject.GetPeriodStatsAsync(123);

      // Assert.
      Assert.IsInstanceOf<OkNegotiatedContentResult<string>>(result);

      var okResult = (OkNegotiatedContentResult<string>)result;
      var stats = JsonConvert.DeserializeObject<GoalPeriodStatsData>(okResult.Content);

      var startOfDay = objects.DateTime.Now;
      startOfDay = new DateTime(startOfDay.Year, startOfDay.Month, startOfDay.Day, 0, 0, 0, DateTimeKind.Utc);
      var endOfDay = new DateTime(startOfDay.Year, startOfDay.Month, startOfDay.Day, 23, 59, 59, DateTimeKind.Utc);

      Assert.AreEqual(123, stats.GoalId);
      Assert.AreEqual(startOfDay, stats.PeriodStart);
      Assert.AreEqual(endOfDay, stats.PeriodEnd);
      Assert.AreEqual(1, stats.AttemptCount);
      Assert.AreEqual(2, stats.TargetAttemptCount);
    }

    //=============================================================================================

    private static TestObjects CreateTestObjects()
    {
      var goals = Substitute.For<IGoalStore>();
      var attempts = Substitute.For<IGoalAttemptStore>();
      var users = Substitute.For<IUserStore>();
      var tokens = Substitute.For<ITokenStore>();
      var dateTime = Substitute.For<IDateTimeSource>();
      var log = Substitute.For<ILogger>();

      return new TestObjects
      {
        Goals = goals,
        Attempts = attempts,
        Users = users,
        Tokens = tokens,
        DateTime = dateTime,
        Log = log,
        TestObject = new GoalController(goals, attempts, users, tokens, dateTime, log)
      };
    }

    //---------------------------------------------------------------------------------------------
  }
}
