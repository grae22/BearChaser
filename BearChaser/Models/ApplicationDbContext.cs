using System.Data.Entity;

namespace BearChaser.Models
{
  internal class ApplicationDbContext : DbContext
  {
    //---------------------------------------------------------------------------------------------

    public DbSet<Setting> Settings { get; set; }
    public DbSet<User> Users { get; set; }

    //---------------------------------------------------------------------------------------------
  }
}