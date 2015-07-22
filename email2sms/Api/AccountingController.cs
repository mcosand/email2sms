namespace email2sms.Api
{
  using System.Configuration;
  using System.Data.Entity;
  using System.Linq;
  using System.Threading.Tasks;
  using System.Web.Http;
  using email2sms.Data;
  using Twilio;

  public class AccountingController : ApiController
  {
    [HttpGet]
    public async Task<string> Reconcile()
    {
      var twilioSid = ConfigurationManager.AppSettings["twilio:sid"];
      var twilioToken = ConfigurationManager.AppSettings["twilio:token"];

      var twilioClient = new TwilioRestClient(twilioSid, twilioToken);

      int count = 0;
      using (var db = new Email2SmsContext())
      {
        var unknowns = await db.InvoiceItems.Where(f => f.Sid != null && f.Price == null).ToListAsync();
        foreach (var entry in unknowns)
        {
          var msg = twilioClient.GetMessage(entry.Sid);
          if (msg.Status == "delivered")
          {
            count++;
            entry.Price = -msg.Price;
          }
        }
        await db.SaveChangesAsync();
      }

      return "OK. Records updated: " + count.ToString();
    }
  }
}
