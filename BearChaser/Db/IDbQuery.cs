using System.Collections.Generic;
using System.Threading.Tasks;

namespace BearChaser.Db
{
  public interface IDbQuery
  {
    //---------------------------------------------------------------------------------------------

    Task<List<T>> ExecuteSql<T>(string sql);

    //---------------------------------------------------------------------------------------------
  }
}
