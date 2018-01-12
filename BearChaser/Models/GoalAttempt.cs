using System;
using Newtonsoft.Json;

namespace BearChaser.Models
{
  public class GoalAttempt
  {
    //---------------------------------------------------------------------------------------------

    public int Id { get; set; }
    public int GoalId { get; set; }
    public virtual Goal Goal { get; set; }
    public DateTime Timestamp { get; set; }

    //---------------------------------------------------------------------------------------------

    public override string ToString()
    {
      return JsonConvert.SerializeObject(this);
    }

    //---------------------------------------------------------------------------------------------
  }
}