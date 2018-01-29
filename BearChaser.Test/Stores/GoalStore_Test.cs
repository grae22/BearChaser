using System;
using System.Threading.Tasks;
using NUnit.Framework;
using NSubstitute;
using BearChaser.Db;
using BearChaser.Models;
using BearChaser.Stores;

namespace BearChaser.Test.Stores
{
  [TestFixture]
  [Category("GoalStore")]
  internal class GoalStore_Test
  {
    //---------------------------------------------------------------------------------------------

    [Test]
    public async Task CreateGoalAsync_GivenValidGoal_ShouldAddToDb()
    {
      // Arrange.
      var goalDb = Substitute.For<IGoalDb>();
      var userStore = Substitute.For<IUserStore>();
      var testObject = new GoalStore(goalDb, userStore);
      var now = DateTime.Now;

      userStore.GetUserAsync(0).Returns(new User());

      // Act.
      Goal goal = await testObject.CreateGoalAsync(0, "SomeGoal", 24, 1, now);

      // Assert.
      goalDb.Received(1).AddGoal(Arg.Any<Goal>());

      await goalDb.Received(1).SaveAsync();

      Assert.AreEqual(0, goal.UserId);
      Assert.AreEqual("SomeGoal", goal.Name);
      Assert.AreEqual(24, goal.PeriodInHours);
      Assert.AreEqual(1, goal.FrequencyWithinPeriod);
      Assert.AreEqual(now, goal.StartDate);
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public void CreateGoalAsync_GivenInvalidUserId_ShouldRaiseException()
    {
      // Arrange.
      var goalDb = Substitute.For<IGoalDb>();
      var userStore = Substitute.For<IUserStore>();
      var testObject = new GoalStore(goalDb, userStore);

      // Act & assert.
      Assert.That(
        () => testObject.CreateGoalAsync(0, "SomeGoal", 24, 1, DateTime.Now),
        Throws
          .TypeOf<ArgumentException>()
          .With.Message.EqualTo("User not found with id 0."));
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    [TestCase(null)]
    [TestCase("  ")]
    public void CreateGoalAsync_GivenNullOrBlankName_ShouldRaiseException(string name)
    {
      // Arrange.
      var goalDb = Substitute.For<IGoalDb>();
      var userStore = Substitute.For<IUserStore>();
      var testObject = new GoalStore(goalDb, userStore);

      userStore.GetUserAsync(0).Returns(new User());

      // Act & assert.
      Assert.That(
        () => testObject.CreateGoalAsync(0, name, 24, 1, DateTime.Now),
        Throws
          .TypeOf<ArgumentException>()
          .With.Message.EqualTo("Goal name cannot be blank or null."));
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public void CreateGoalAsync_GivenInvalidPeriod_ShouldRaiseException()
    {
      // Arrange.
      var goalDb = Substitute.For<IGoalDb>();
      var userStore = Substitute.For<IUserStore>();
      var testObject = new GoalStore(goalDb, userStore);

      userStore.GetUserAsync(0).Returns(new User());

      // Act & assert.
      Assert.That(
        () => testObject.CreateGoalAsync(0, "SomeGoal", 0, 1, DateTime.Now),
        Throws
          .TypeOf<ArgumentException>()
          .With.Message.EqualTo("Invalid period value 0."));
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    [TestCase(0)]
    [TestCase(-1)]
    public void CreateGoalAsync_GivenZeroOrNegativeFrequency_ShouldRaiseException(int frequency)
    {
      // Arrange.
      var goalDb = Substitute.For<IGoalDb>();
      var userStore = Substitute.For<IUserStore>();
      var testObject = new GoalStore(goalDb, userStore);

      userStore.GetUserAsync(0).Returns(new User());

      // Act & assert.
      Assert.That(
        () => testObject.CreateGoalAsync(0, "SomeGoal", 24, frequency, DateTime.Now),
        Throws
          .TypeOf<ArgumentException>()
          .With.Message.EqualTo($"Frequency cannot be zero or negative, was {frequency}."));
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public async Task RemoveGoalAsync_GivenGoal_ShouldRemoveFromDb()
    {
      // Arrange.
      var goalDb = Substitute.For<IGoalDb>();
      var userStore = Substitute.For<IUserStore>();
      var testObject = new GoalStore(goalDb, userStore);
      var goalToRemove = new Goal();

      // Act.
      await testObject.RemoveGoalAsync(goalToRemove);

      // Assert.
      goalDb.Received(1).RemoveGoal(goalToRemove);

      await goalDb.Received(1).SaveAsync();
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public async Task GetGoalsAsync_GivenUserId_ShouldQueryDb()
    {
      // Arrange.
      var goalDb = Substitute.For<IGoalDb>();
      var userStore = Substitute.For<IUserStore>();
      var testObject = new GoalStore(goalDb, userStore);

      // Act.
      await testObject.GetGoalsAsync(0);

      // Assert.
      await goalDb.Received(1).GetGoalsAsync(0);
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public async Task GetGoalAsync_GivenGoalId_ShouldQueryDb()
    {
      // Arrange.
      var goalDb = Substitute.For<IGoalDb>();
      var userStore = Substitute.For<IUserStore>();
      var testObject = new GoalStore(goalDb, userStore);

      // Act.
      await testObject.GetGoalAsync(0);

      // Assert.
      await goalDb.Received(1).GetGoalAsync(0);
    }

    //---------------------------------------------------------------------------------------------
  }
}
