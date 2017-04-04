using System;
using System.Configuration;
using System.Linq;
using System.Web.Http;
using email2sms.Api.Model;
using email2sms.Data;
using Stripe;

namespace email2sms.Api
{
  public class StripeController : ApiController
  {
    [HttpPost]
    [Route("api/stripe/hook")]
    public object StripeHook(StripeMessage<StripeCreateInvoice> message)
    {
      Metrics.Info($"Received stripe webhook call: {message.Type}");
      if (message.Type == "invoice.created") return InvoiceCreated(message.Data.Object);
      return "NOOP";
    }

    private object InvoiceCreated(StripeCreateInvoice invoiceData)
    {
      using (var db = new Email2SmsContext())
      {
        var sub = db.Subscriptions.Where(f => f.StripeCustomer == invoiceData.Customer).FirstOrDefault();
        if (sub == null)
        {
          Metrics.Error($"Tried to create invoice for {invoiceData.Customer}, who doesn't seem to be a subscriber");
          return "Not Known";
        }

        var end = DateTimeOffset.FromUnixTimeSeconds(invoiceData.Period_End).UtcDateTime;

        var charges = db.InvoiceItems.Where(
          f => f.SendTime < end &&
          f.SendTime >= f.SendTo.Subscription.LastInvoiceUtc &&
          f.SendTo.Subscription.StripeCustomer == invoiceData.Customer
          ).Select(f => f.Price).ToArray();
        var sum = (int)((charges.Sum() ?? 0.0M) * 100);
        if (sum > 0)
        {
          var stripe = new StripeInvoiceItemService(ConfigurationManager.AppSettings["stripe:token_secret"]);
          Metrics.Info($"Charging {sub.StripeCustomer} ${charges.Sum().Value} for {charges.Length} messages");
          var response = stripe.Create(new StripeInvoiceItemCreateOptions
          {
            Amount = sum,
            Currency = "usd",
            CustomerId = invoiceData.Customer,
            Description = $"Messages x {charges.Length}"
          });
        }

        sub.LastInvoiceUtc = end;
        db.SaveChanges();
      }
      return "OK";
    }
  }
}
