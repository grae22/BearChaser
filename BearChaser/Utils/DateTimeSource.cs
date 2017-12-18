using System;

namespace BearChaser.Utils
{
  internal class DateTimeSource : IDateTimeSource
  {
    public DateTime Now
    {
      get
      {
        return DateTime.Now;
      }
    }
  }
}
