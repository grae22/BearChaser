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
    public DateTime? LastAttemptDate { get; set; }
    public int AverageCompletionAcrossAllPeriods { get; set; }
    public int AverageCompletionAcrossLast3Periods { get; set; }

    //---------------------------------------------------------------------------------------------
  }
}