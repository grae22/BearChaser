using BearChaser.Utils;
using NUnit.Framework;

namespace BearChaser.Test.Utils
{
  [TestFixture]
  [Category("StringExtensions")]
  internal class StringExtensions_Test
  {
    //---------------------------------------------------------------------------------------------

    [Test]
    public void GetAsPasswordHash_GivenTwoIdenticalStrings_ShouldReturnSameHash()
    {
      // Arrange.
      var string1 = "SomeString";
      var string2 = string1;

      // Act.
      int hash1 = string1.GetAsPasswordHash();
      int hash2 = string2.GetAsPasswordHash();

      // Assert.
      Assert.AreEqual(hash1, hash2);
    }

    //---------------------------------------------------------------------------------------------

    [Test]
    public void GetAsPasswordHash_GivenTwoDifferentStrings_ShouldReturnDifferentHashes()
    {
      // Arrange.
      var string1 = "SomeString";
      var string2 = "SomeOtherString";

      // Act.
      int hash1 = string1.GetAsPasswordHash();
      int hash2 = string2.GetAsPasswordHash();

      // Assert.
      Assert.AreNotEqual(hash1, hash2);
    }

    //---------------------------------------------------------------------------------------------
  }
}
