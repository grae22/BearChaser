// TODO: Something needs to clear out expired tokens from time to time.

using System;
using Newtonsoft.Json;

namespace BearChaser.Models
{
  public class Token
  {
    //---------------------------------------------------------------------------------------------

    public int Id { get; set; }
    public Guid Value { get; set; }
    public DateTime Expiry { get; set; } = new DateTime(2000, 1, 1);

    //---------------------------------------------------------------------------------------------

    public override string ToString()
    {
      return JsonConvert.SerializeObject(this);
    }

    //---------------------------------------------------------------------------------------------
  }
}