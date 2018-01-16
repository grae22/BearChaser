using System.Data.Entity;
using NUnit.Framework;
using NSubstitute;
using BearChaser.Models;
using BearChaser.Stores;
using BearChaser.Utils.Logging;

namespace BearChaser.Test.Stores
{
  [TestFixture]
  [Category("SettingsStore")]
  internal class SettingsStore_Test
  {
    //---------------------------------------------------------------------------------------------

    [Test]
    public void GetValue_GivenExistingStringSetting_ShouldReturnValue()
    {
      // Arrange.
      var settingsFromDb = Substitute.For<IDbSet<Setting>>();
      settingsFromDb.Find(new[] { "SomeKey" }).Returns(new Setting { Id = "SomeKey", Value = "SomeValue" });

      var log = Substitute.For<ILogger>();

      var testObject = new SettingsStore(settingsFromDb, log);

      // Act.
      var value = testObject.GetValue("SomeKey", null);

      // Assert.
      Assert.AreEqual("SomeValue", value);
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public void GetValue_GivenExistingIntSetting_ShouldReturnValue()
    {
      // Arrange.
      var settingsFromDb = Substitute.For<IDbSet<Setting>>();
      settingsFromDb.Find(new[] { "SomeKey" }).Returns(new Setting { Id = "SomeKey", Value = "123" });

      var log = Substitute.For<ILogger>();

      var testObject = new SettingsStore(settingsFromDb, log);

      // Act.
      var value = testObject.GetValue("SomeKey", 666);

      // Assert.
      Assert.AreEqual(123, value);
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public void GetValue_GivenNonExistingSetting_ShouldReturnDefaultValue()
    {
      // Arrange.
      var settingsFromDb = Substitute.For<IDbSet<Setting>>();
      settingsFromDb.Find(new[] { "SomeKey" }).Returns((Setting)null);

      var log = Substitute.For<ILogger>();

      var testObject = new SettingsStore(settingsFromDb, log);

      // Act.
      var value = testObject.GetValue("SomeKey", "Default");

      // Assert.
      Assert.AreEqual("Default", value);
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public void GetValue_GivenIntSettingWithInvalidValue_ShouldReturnDefaultValueAndLogError()
    {
      // Arrange.
      var settingsFromDb = Substitute.For<IDbSet<Setting>>();
      settingsFromDb.Find(new[] { "SomeKey" }).Returns(new Setting { Id = "SomeKey", Value = "123x" });

      var log = Substitute.For<ILogger>();

      var testObject = new SettingsStore(settingsFromDb, log);

      // Act.
      var value = testObject.GetValue("SomeKey", 666);

      // Assert.
      Assert.AreEqual(666, value);

      log.Received(1).LogError(
        "Setting \"SomeKey\" contains an invalid integer value \"123x\". Using default value \"666\".",
        Arg.Any<bool>(),
        Arg.Any<string>(),
        Arg.Any<string>(),
        Arg.Any<int>());
    }

    //---------------------------------------------------------------------------------------------
  }
}
