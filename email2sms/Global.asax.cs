﻿using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using log4net;
using log4net.Config;

namespace email2sms
{
  public class MvcApplication : System.Web.HttpApplication
  {
    protected void Application_Start()
    {
      XmlConfigurator.Configure();

      AreaRegistration.RegisterAllAreas();
      GlobalConfiguration.Configure(WebApiConfig.Register);
      FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
      RouteConfig.RegisterRoutes(RouteTable.Routes);
      BundleConfig.RegisterBundles(BundleTable.Bundles);
    }

    protected void Application_Error()
    {
      LogManager.GetLogger("Errors").Error(Server.GetLastError());
    }
  }
}
