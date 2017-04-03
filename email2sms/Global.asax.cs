using System;
using System.Web.Http;
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
      var ex = Server.GetLastError();
      LogManager.GetLogger("Errors").Error(ex);
      using (var db = new email2sms.Data.Email2SmsContext())
      {
        db.Errors.Add(new Data.ErrorRow { User = User.Identity.Name, TimeUtc = DateTime.UtcNow, Message = ex.Message, Stack = ex.ToString() });
        db.SaveChanges();
      }
    }
  }
}
