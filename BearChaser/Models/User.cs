namespace BearChaser.Models
{
  public class User
  {
    //---------------------------------------------------------------------------------------------

    public int Id { get; set; }
    public string Username { get; set; }
    public int Password { get; set; }
    public int? TokenId { get; set; }
    public virtual Token Token { get; set; }

    //---------------------------------------------------------------------------------------------
  }
}