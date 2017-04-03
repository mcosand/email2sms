using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Filters;
using log4net;

namespace email2sms
{
  public static class WebApiConfig
  {
    public static void Register(HttpConfiguration config)
    {
      // Web API configuration and services

      // Web API routes
      config.MapHttpAttributeRoutes();
      config.Filters.Add(new ExceptionHandlingAttribute());
      //config.Routes.MapHttpRoute(
      //    name: "DefaultApi",
      //    routeTemplate: "api/{controller}/{id}",
      //    defaults: new { id = RouteParameter.Optional }
      //);
    }
  }

  public class ExceptionHandlingAttribute : ExceptionFilterAttribute
  {
    public override void OnException(HttpActionExecutedContext context)
    {
      //Log Critical errors
      LogManager.GetLogger("Errors").Error(context.Exception);

      throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
      {
        Content = new StringContent("An error occurred, please try again or contact the administrator."),
        ReasonPhrase = "Critical Exception"
      });
    }
  }
}
