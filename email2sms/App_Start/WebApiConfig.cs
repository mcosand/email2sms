using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Filters;

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
      Metrics.Exception(context.Exception);

      //Log Critical errors
      using (var db = new email2sms.Data.Email2SmsContext())
      {
        db.Errors.Add(new Data.ErrorRow { User = context.ActionContext.RequestContext.Principal.Identity.Name,
          TimeUtc = DateTime.UtcNow, Message = context.Exception.Message, Stack = context.Exception.ToString() });
        db.SaveChanges();
      }

      throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
      {
        Content = new StringContent("An error occurred, please try again or contact the administrator."),
        ReasonPhrase = "Critical Exception"
      });
    }
  }
}
