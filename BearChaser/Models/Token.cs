using System;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace BearChaser.Models
{
  public class Token
  {
    //---------------------------------------------------------------------------------------------

    public int Id { get; set; }

    [Index(IsUnique = true)]
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