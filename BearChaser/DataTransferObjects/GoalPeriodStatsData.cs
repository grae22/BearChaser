using System;

namespace BearChaser.DataTransferObjects
{
  public class GoalPeriodStatsData
  {
    //---------------------------------------------------------------------------------------------

    public int GoalId { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public int AttemptCount { get; set; }
    public int TargetAttemptCount { get; set; }

    //---------------------------------------------------------------------------------------------
  }
}