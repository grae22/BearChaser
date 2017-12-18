using System;

namespace BearChaser.Utils
{
  public interface IDateTimeSource
  {
    DateTime Now { get; }
  }
}
