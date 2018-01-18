using System;

namespace BearChaser.DataTransferObjects
{
  public class GoalAttemptData
  {
    //---------------------------------------------------------------------------------------------

    public int Id { get; set; }
    public int GoalId { get; set; }
    public DateTime Timestamp { get; set; }

    //---------------------------------------------------------------------------------------------
  }
}