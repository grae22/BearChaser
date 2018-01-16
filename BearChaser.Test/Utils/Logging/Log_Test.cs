using NUnit.Framework;
using NSubstitute;
using BearChaser.Utils.Logging;

namespace BearChaser.Test.Utils.Logging
{
  [TestFixture]
  [Category("Log")]
  internal class Log_Test
  {
    //---------------------------------------------------------------------------------------------

    [Test]
    public void LogDebug_GivenLogDebugCalled_ShouldCallLogDebugOnRegisteredLogger()
    {
      // Arrange.
      var testObject = new Log();
      var log1 = Substitute.For<ILogger>();
      var log2 = Substitute.For<ILogger>();

      testObject.RegisterLogger(log1);
      testObject.RegisterLogger(log2);

      // Act.
      testObject.LogDebug("Message");

      // Assert.
      log1.Received(1).LogDebug("Message", false, Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>());
      log2.Received(1).LogDebug("Message", false, Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>());
    }

    //---------------------------------------------------------------------------------------------
    
    [Test]
    public void LogInfog_GivenLogInfoCalled_ShouldCallLogInfoOnRegisteredLogger()
    {
      // Arrange.
      var testObject = new Log();
      var log1 = Substitute.For<ILogger>();
      var log2 = Substitute.For<ILogger>();

      testObject.RegisterLogger(log1);
      testObject.RegisterLogger(log2);

      // Act.
      testObject.LogInfo("Message");

      // Assert.
      log1.Received(1).LogInfo("Message", false, Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>());
      log2.Received(1).LogInfo("Message", false, Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>());
    }

    //---------------------------------------------------------------------------------------------    

    [Test]
    public void LogWarning_GivenLogWarningCalled_ShouldCallLogWarningOnRegisteredLogger()
    {
      // Arrange.
      var testObject = new Log();
      var log1 = Substitute.For<ILogger>();
      var log2 = Substitute.For<ILogger>();

      testObject.RegisterLogger(log1);
      testObject.RegisterLogger(log2);

      // Act.
      testObject.LogWarning("Message");

      // Assert.
      log1.Received(1).LogWarning("Message", false, Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>());
      log2.Received(1).LogWarning("Message", false, Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>());
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public void LogError_GivenLogDebugCalled_ShouldCallLogErrorOnRegisteredLogger()
    {
      // Arrange.
      var testObject = new Log();
      var log1 = Substitute.For<ILogger>();
      var log2 = Substitute.For<ILogger>();

      testObject.RegisterLogger(log1);
      testObject.RegisterLogger(log2);

      // Act.
      testObject.LogError("Message");

      // Assert.
      log1.Received(1).LogError("Message", true, Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>());
      log2.Received(1).LogError("Message", true, Arg.Any<string>(), Arg.Any<string>(), Arg.Any<int>());
    }

    //---------------------------------------------------------------------------------------------
  }
}
