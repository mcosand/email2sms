using System;
using System.Configuration;
using System.Linq;
using System.Web.Http;
using email2sms.Api.Model;
using email2sms.Data;
using Newtonsoft.Json;
using Twilio;

namespace email2sms.Api
{
    public class SinkController : ApiController
    {
      public object Post(EmailMessage message)
      {
        try
        {
          var twilioSid = ConfigurationManager.AppSettings["twilio:sid"];
          var twilioToken = ConfigurationManager.AppSettings["twilio:token"];
          var twilioFrom = ConfigurationManager.AppSettings["twilio:numbers"];

          bool hasTwilio = !string.IsNullOrWhiteSpace(twilioSid);
          hasTwilio &= !string.IsNullOrWhiteSpace(twilioToken);
          hasTwilio &= !string.IsNullOrWhiteSpace(twilioFrom);

          if (hasTwilio)
          {
            var twilioClient = new TwilioRestClient(twilioSid, twilioToken);
            var twilioFroms = twilioFrom.Split(',');
            var fromIndex = 0;

            using (var db = new Email2SmsContext())
            {
              var msgLog = GetMessageLog(message);
              db.MessageLog.Add(msgLog);
              db.SaveChanges();

              DateTime duplicateTime = DateTime.UtcNow.AddMinutes(-5);
              if (db.InvoiceItems.Any(f => f.Message.Text == message.plain && f.SendTime > duplicateTime))
              {
                return "Duplicate";
              }

              var list = db.Phones.ToList();
              foreach (var item in list)
              {
                var twilioMsg = twilioClient.SendMessage(twilioFroms[fromIndex], item.Address, message.plain);
                fromIndex = (fromIndex + 1) % twilioFroms.Length;
                db.InvoiceItems.Add(new InvoiceLog { SendTo = item, Price = twilioMsg.Price, SendTime = DateTime.UtcNow, Message = msgLog });
                db.SaveChanges();
              }
            }
          }
        } catch (Exception ex)
        {
          // debugger here:
          throw;
        }
        return "OK";
      }

      private static MessageLog GetMessageLog(EmailMessage message)
      {
        var msg = new MessageLog
        {
          Received = DateTime.UtcNow,
          Json = JsonConvert.SerializeObject(message),
          Text = message.plain
        };

        return msg;
      }
    }
}
