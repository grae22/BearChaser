using Newtonsoft.Json;

namespace BearChaser.Models
{
  public class Goal
  {
    //---------------------------------------------------------------------------------------------

    public enum TimePeriod
    {
      Day,
      Week,
      Fortnight,
      Month,
      Quarter,
      HalfYear,
      Year
    }

    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public string Name { get; set; }
    public TimePeriod Period { get; set; }
    public int FrequencyWithinPeriod { get; set; }

    //---------------------------------------------------------------------------------------------

    public override string ToString()
    {
      return JsonConvert.SerializeObject(this);
    }

    //---------------------------------------------------------------------------------------------
  }
}