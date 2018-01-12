using System;
using System.Threading.Tasks;
using System.Web.Http.Results;
using NUnit.Framework;
using NSubstitute;
using BearChaser.Controllers;
using BearChaser.DataTransferObjects;
using BearChaser.Models;
using BearChaser.Stores;
using BearChaser.Utils.Logging;

namespace BearChaser.Test.Controllers
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
    public async Task CreateGoalAsync_GivenValidParams_ShouldAddGoalToStore()
    {
      // Arrange.
      var goalStore = Substitute.For<IGoalStore>();
      var userStore = Substitute.For<IUserStore>();
      var tokenStore = Substitute.For<ITokenStore>();
      var log = Substitute.For<ILogger>();
      var testObject = new GoalController(goalStore, userStore, tokenStore, log);
      var guid = Guid.NewGuid();
      var userToken = new Token();

      tokenStore.GetExistingValidTokenByGuidAsync(guid).Returns(userToken);
      userStore.GetUserAsync(userToken).Returns(new User { Id = 123 });

      // Act.
      var result = await testObject.CreateGoalAsync(
        new GoalData
        {
          UserToken = guid.ToString(),
          Name = "NewGoal",
          Period = 1,
          FrequencyWithinPeriod = 2
        });

      // Assert.
      await goalStore.Received(1).CreateGoalAsync(123, "NewGoal", (Goal.TimePeriod)1, 2);

      Assert.IsInstanceOf<OkResult>(result);
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

      // Act.
      var result = await testObject.CreateGoalAsync(
        new GoalData
        {
          UserToken = "Not A Guid",
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

      tokenStore.GetExistingValidTokenByGuidAsync(guid).Returns((Token)null);

      // Act.
      var result = await testObject.CreateGoalAsync(
        new GoalData
        {
          UserToken = guid.ToString(),
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

      tokenStore.GetExistingValidTokenByGuidAsync(guid).Returns(userToken);
      userStore.GetUserAsync(userToken).Returns((User)null);

      // Act.
      var result = await testObject.CreateGoalAsync(
        new GoalData
        {
          UserToken = guid.ToString(),
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
