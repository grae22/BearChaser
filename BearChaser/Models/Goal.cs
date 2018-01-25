using System;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace BearChaser.Models
{
  public class Goal
  {
    //---------------------------------------------------------------------------------------------
    
    public int Id { get; set; }
    public int UserId { get; set; }
    public virtual User User { get; set; }
    public string Name { get; set; }
    public int PeriodInHours { get; set; }
    public int FrequencyWithinPeriod { get; set; }

    [Column(TypeName = "DateTime2")]
    public DateTime StartDate { get; set; }

    //---------------------------------------------------------------------------------------------

    public override string ToString()
    {
      return JsonConvert.SerializeObject(this);
    }

    //---------------------------------------------------------------------------------------------
  }
}