using System;

namespace BearChaser.DataTransferObjects
{
  public class GoalData
  {
    //---------------------------------------------------------------------------------------------

    public int Id { get; set; }
    public string Name { get; set; }
    public int PeriodInHours { get; set; }
    public int FrequencyWithinPeriod { get; set; }
    public DateTime StartDate { get; set; }

    //---------------------------------------------------------------------------------------------
  }
}