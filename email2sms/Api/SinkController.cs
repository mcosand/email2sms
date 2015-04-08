using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
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
          var twilioCost = decimal.Parse(ConfigurationManager.AppSettings["twilio:cost"] ?? ".0075");
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

              var list = db.Phones.ToList();
              foreach (var item in list)
              {
               // var twilioMsg = twilioClient.SendMessage(twilioFroms[fromIndex], item.Address, message.plain);
                fromIndex = (fromIndex + 1) % twilioFroms.Length;
                Message twilioMsg = new Message { Price = 0.0075M };
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
          Json = JsonConvert.SerializeObject(message)
        };

        return msg;
      }
    }
}
