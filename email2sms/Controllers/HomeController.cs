using System;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using email2sms.Data;
using email2sms.Models;
using Stripe;

namespace email2sms.Controllers
{
  public class HomeController : Controller
  {
    public ActionResult Index()
    {
      return View();
    }
    protected override void OnException(ExceptionContext filterContext)
    {
      Metrics.Exception(new InvalidOperationException("controller handler " + filterContext.Exception.Message, filterContext.Exception));
      base.OnException(filterContext);
    }

    private Guid GetUserId()
    {
      var user = (ClaimsPrincipal)User;
      return new Guid(user.FindFirst(ClaimTypes.NameIdentifier).Value);
    }

    [Authorize]
    [HttpGet]
    public ActionResult Setup()
    {
      Metrics.Info("Tracing information");
      using (var db = new Email2SmsContext())
      {
        var userId = GetUserId();
        var data = db.Subscriptions.Where(f => f.User == userId)
          .Select(f => new { f, Phones = f.Phones })
          .AsNoTracking()
          .FirstOrDefault();

        return View(new SetupInfoViewModel
        {
          Email = User.Identity.Name,
          Customer = data?.f,
          Phones = data?.Phones?.Where(f => f.Active)?.Select(f => f.Address)?.ToArray() ?? new string[0],
          Inactive = data?.Phones?.Where(f => f.Active)?.Select(f => f.Address)?.ToArray() ?? new string[0]
        });
      }
    }

    [Authorize]
    [HttpPost]
    public ActionResult Setup(string phones)
    {
      string[] list = phones.Split('\n').Select(f => f.Trim()).Distinct().Where(f => !string.IsNullOrWhiteSpace(f)).ToArray();
      string message = null;

      for (int i = 0; i < list.Length; i++)
      {
        if (list[i].Length != 10 || !Regex.IsMatch(list[i], "\\d{10}"))
        {
          message = (message ?? "Error: ") + " Don't understand " + list[i];
        }
      }
      if (message != null)
      {
        Metrics.Info(ViewBag.Message);
        ViewBag.Message = message;
        return Setup();
      }

      using (var db = new Email2SmsContext())
      {
        var userId = GetUserId();

        var dupes = db.Phones
          .Where(f => f.Subscription.User != userId)
          .Select(f => f.Address)
          .ToArray()
          .GroupJoin(list, f => f, f => f, (a, b) => a)
          .ToArray();
        if (dupes.Length > 0)
        {
          ViewBag.Message = "Error: Phone numbers already used by someone else: " + string.Join(", ", dupes);
          Metrics.Info(ViewBag.Message);
          return Setup();
        }

        var sub = db.Subscriptions.Include(f => f.Phones).SingleOrDefault(f => f.User == userId);
        foreach (var p in list)
        {
          var match = sub.Phones.FirstOrDefault(f => f.Address == p);
          if (match == null)
          {
            if (TwilioProvider.HasTwilio())
            {
              var client = TwilioProvider.GetTwilio();

              Metrics.Info($"Sending welcome text to {p}");
              client.SendMessage(TwilioProvider.GetNumbers().First(), p, "This number will now receive SAR pages. matt@cosand.net");
            }
            sub.Phones.Add(new Phone { Active = true, Address = p, Subscription = sub });
          }
          else if (match.Active == false)
          {
            Metrics.Info($"Marking {p} as active");
            match.Active = true;
          }
        }

        foreach (var p in sub.Phones.ToArray())
        {
          var match = list.FirstOrDefault(f => f == p.Address);
          if (match == null)
          {
            if (db.InvoiceItems.Any(f => f.PhoneId == p.Id))
            {
              Metrics.Info($"Marking phone {p.Address} inactive");
              p.Active = false;
            }
            else
            {
              Metrics.Info($"Removing unused phone {match}");
              sub.Phones.Remove(p);
              db.Phones.Remove(p);
            }
          }
          else
          {
            // Already marked active in above loop
          }
        }
        db.SaveChanges();
      }

      ViewBag.Message = $"Saved {list.Count()} numbers";
      return Setup();
    }

    [HttpGet]
    [Authorize]
    public ActionResult History()
    {
      using (var db = new Email2SmsContext())
      {
        var userId = GetUserId();

        return View(db.InvoiceItems.Where(f => f.SendTo.Subscription.User == userId).Select(f => new HistoryRow
        {
          Phone = f.SendTo.Address,
          Message = f.Message.Text,
          When = f.SendTime,
          Cost = f.Price
        })
        .OrderByDescending(f => f.When)
        .Take(50)
        .ToArray());
      }
    }

    [HttpPost]
    [Authorize]
    public ActionResult Subscribe(StripeTokenResponse data)
    {
      var myCustomer = new StripeCustomerCreateOptions();
      myCustomer.Email = User.Identity.Name;
      myCustomer.SourceToken = data.StripeToken;
      myCustomer.PlanId = ConfigurationManager.AppSettings["stripe:plan"];                          // only if you have a plan
      myCustomer.TaxPercent = 0;                            // only if you are passing a plan, this tax percent will be added to the price.

      Metrics.Info($"Creating subscription for {myCustomer.Email} {myCustomer.SourceToken} {myCustomer.PlanId}");


      var customerService = new StripeCustomerService(ConfigurationManager.AppSettings["stripe:token_secret"]);
      StripeCustomer stripeCustomer = customerService.Create(myCustomer);


      using (var db = new Email2SmsContext())
      {
        var userId = GetUserId();
        var row = db.Subscriptions.FirstOrDefault(f => f.User == userId);
        if (row == null)
        {
          row = new Subscription { User = userId };
          db.Subscriptions.Add(row);
        }
        row.StripeCustomer = stripeCustomer.Id;
        row.LastInvoiceUtc = DateTime.UtcNow;
        db.SaveChanges();
      }

      return Redirect("~/home/setup");
    }

    public ContentResult TestErrors()
    {
      throw new ApplicationException("test exceptions");
    }
  }
}