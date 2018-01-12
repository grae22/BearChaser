using System.Web.Http;
using Unity;
using BearChaser.Models;
using BearChaser.Settings;
using BearChaser.Stores;
using BearChaser.Utils;
using BearChaser.Utils.Logging;

namespace BearChaser
{
  public static class WebApiConfig
  {
    //---------------------------------------------------------------------------------------------

    public static void Register(HttpConfiguration config)
    {
      // Web API configuration and services
      SetupContainer(config);

      // Web API routes
      config.MapHttpAttributeRoutes();
    }

    //---------------------------------------------------------------------------------------------
    
    private static void SetupContainer(HttpConfiguration config)
    {
      var log = new Log();
      var consoleLogger = new ConsoleLogger();
      log.RegisterLogger(consoleLogger);

      var settings = new AllSettings(new ApplicationDbContext().Settings, log);

      var fileLogger = new FileLogger(settings);
      log.RegisterLogger(fileLogger);

      var container = new UnityContainer();
      config.DependencyResolver = new UnityResolver(container);
      
      // Types.
      container.RegisterType<IUserStore, UserStore>();
      container.RegisterType<ITokenDb, ApplicationDbContext>();
      container.RegisterType<ITokenStore, TokenStore>();
      container.RegisterType<IGoalDb, ApplicationDbContext>();
      container.RegisterType<IGoalStore, GoalStore>();

      // Instances.
      container.RegisterInstance<IDateTimeSource>(new DateTimeSource());
      container.RegisterInstance<IUserSettings>(settings);
      container.RegisterInstance<ITokenSettings>(settings);
      container.RegisterInstance<ILogger>(log);
    }

    //---------------------------------------------------------------------------------------------
  }
}
