using System.Web.Mvc;

namespace BearChaser.Controllers
{
  public class HomeController : Controller
  {
    //---------------------------------------------------------------------------------------------

    public ActionResult Index()
    {
      ViewBag.Title = "BearChaserAPI";

      return View();
    }

    //---------------------------------------------------------------------------------------------
  }
}
