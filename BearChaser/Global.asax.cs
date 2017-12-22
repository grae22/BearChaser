using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using BearChaser.Controllers;
using BearChaser.Factories;
using BearChaser.Models;
using BearChaser.Settings;
using BearChaser.Stores;
using BearChaser.Utils;
using BearChaser.Utils.Logging;

namespace BearChaser
{
  public class WebApiApplication : System.Web.HttpApplication
  {
    //---------------------------------------------------------------------------------------------

    protected void Application_Start()
    {
      SetupControllerFactory();

      AreaRegistration.RegisterAllAreas();
      GlobalConfiguration.Configure(WebApiConfig.Register);
      FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
      RouteConfig.RegisterRoutes(RouteTable.Routes);
      BundleConfig.RegisterBundles(BundleTable.Bundles);
    }

    //---------------------------------------------------------------------------------------------

    private static void SetupControllerFactory()
    {
      var log = new Log();
      var consoleLogger = new ConsoleLogger();
      log.RegisterLogger(consoleLogger);

      var settings = new AllSettings(new ApplicationDbContext().Settings, log);

      var fileLogger = new FileLogger(settings);
      log.RegisterLogger(fileLogger);

      var controllerFactory = new ControllerFactory();
      controllerFactory.RegisterController<UserController>("user");

      // Types.
      controllerFactory.RegisterType<IUserStore, UserStore>();
      controllerFactory.RegisterType<ITokenDb, ApplicationDbContext>();
      controllerFactory.RegisterType<ITokenStore, TokenStore>();

      // Instances.
      controllerFactory.RegisterInstance<IDateTimeSource, DateTimeSource>(new DateTimeSource());
      controllerFactory.RegisterInstance<IUserSettings, AllSettings>(settings);
      controllerFactory.RegisterInstance<ITokenSettings, AllSettings>(settings);
      controllerFactory.RegisterInstance<ILogger, Log>(log);

      ControllerBuilder.Current.SetControllerFactory(controllerFactory);
    }

    //---------------------------------------------------------------------------------------------
  }
}
