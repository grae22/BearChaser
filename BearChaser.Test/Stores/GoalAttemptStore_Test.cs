using System;
using System.Threading.Tasks;
using NUnit.Framework;
using NSubstitute;
using BearChaser.Db;
using BearChaser.Models;
using BearChaser.Stores;
using BearChaser.Utils;

namespace BearChaser.Test.Stores
{
  [TestFixture]
  [Category("GoalAttemptStore")]
  internal class GoalAttemptStore_Test
  {
    //---------------------------------------------------------------------------------------------

    [Test]
    public async Task CreateAttemptAsync_GivenValidAttempt_ShouldAddToDb()
    {
      // Arrange.
      var attemptDb = Substitute.For<IGoalAttemptDb>();
      var goalStore = Substitute.For<IGoalStore>();
      var dateTimeSource = Substitute.For<IDateTimeSource>();
      var testObject = new GoalAttemptStore(attemptDb, goalStore, dateTimeSource);

      goalStore.GetGoalAsync(123).Returns(new Goal());
      dateTimeSource.Now.Returns(DateTime.Now);

      // Act.
      GoalAttempt attempt = await testObject.CreateAttemptAsync(123);

      // Assert.
      attemptDb.Received(1).AddAttempt(Arg.Any<GoalAttempt>());

      await attemptDb.Received(1).SaveAsync();

      Assert.AreEqual(123, attempt.GoalId);
      Assert.AreEqual(dateTimeSource.Now, attempt.Timestamp);
    }

    //---------------------------------------------------------------------------------------------
    
    [Test]
    public void CreateAttemptAsync_GivenInvalidGoalId_ShouldRaiseException()
    {
      // Arrange.
      var attemptDb = Substitute.For<IGoalAttemptDb>();
      var goalStore = Substitute.For<IGoalStore>();
      var dateTimeSource = Substitute.For<IDateTimeSource>();
      var testObject = new GoalAttemptStore(attemptDb, goalStore, dateTimeSource);

      // Act & assert.
      Assert.That(
        () => testObject.CreateAttemptAsync(0),
        Throws
          .TypeOf<ArgumentException>()
          .With.Message.EqualTo("Goal not found with id 0."));
    }

    //---------------------------------------------------------------------------------------------
    
    [Test]
    public async Task RemoveAttemptAsync_GivenAttempt_ShouldRemoveFromDb()
    {
      // Arrange.
      var attemptDb = Substitute.For<IGoalAttemptDb>();
      var goalStore = Substitute.For<IGoalStore>();
      var dateTimeSource = Substitute.For<IDateTimeSource>();
      var testObject = new GoalAttemptStore(attemptDb, goalStore, dateTimeSource);
      var attemptToRemove = new GoalAttempt();

      // Act.
      await testObject.RemoveAttemptAsync(123);

      // Assert.
      await attemptDb.Received(1).RemoveAttempt(123);
      await attemptDb.Received(1).SaveAsync();
    }
    
    //---------------------------------------------------------------------------------------------

    [Test]
    public void GetAttemptsAsync_GivenGoalId_ShouldQueryDb()
    {
      // Arrange.
      var attemptDb = Substitute.For<IGoalAttemptDb>();
      var goalStore = Substitute.For<IGoalStore>();
      var dateTimeSource = Substitute.For<IDateTimeSource>();
      var testObject = new GoalAttemptStore(attemptDb, goalStore, dateTimeSource);

      // Act.
      testObject.GetAttempts(0);

      // Assert.
      attemptDb.Received(1).GetAttempts(0);
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public async Task GetAttemptAsync_GivenAttemptId_ShouldQueryDb()
    {
      // Arrange.
      var attemptDb = Substitute.For<IGoalAttemptDb>();
      var goalStore = Substitute.For<IGoalStore>();
      var dateTimeSource = Substitute.For<IDateTimeSource>();
      var testObject = new GoalAttemptStore(attemptDb, goalStore, dateTimeSource);

      // Act.
      await testObject.GetAttemptAsync(123);

      // Assert.
      await attemptDb.Received(1).GetAttemptAsync(123);
    }

    //---------------------------------------------------------------------------------------------
  }
}
