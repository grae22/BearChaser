﻿using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace BearChaser
{
  public class WebApiApplication : System.Web.HttpApplication
  {
    //---------------------------------------------------------------------------------------------

    protected void Application_Start()
    {
      MapperConfig.Initialise();
      AreaRegistration.RegisterAllAreas();
      GlobalConfiguration.Configure(WebApiConfig.Register);
      FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
      RouteConfig.RegisterRoutes(RouteTable.Routes);
      BundleConfig.RegisterBundles(BundleTable.Bundles);
    }

    //---------------------------------------------------------------------------------------------
  }
}
