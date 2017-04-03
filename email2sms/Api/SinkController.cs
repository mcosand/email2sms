﻿using System;
using System.Configuration;
using System.Linq;
using System.Runtime.Caching;
using System.Text.RegularExpressions;
using System.Web.Http;
using email2sms.Api.Model;
using email2sms.Data;
using Newtonsoft.Json;
using Twilio;

namespace email2sms.Api
{
  public class SinkController : ApiController
  {
    static MemoryCache messageCache = new MemoryCache("pagesCache");
    static object cacheLock = new object();

    [Route("api/sink")]
    public object Post(EmailMessage message)
    {
      try
      {
        using (var db = new Email2SmsContext())
        {
          var msgLog = GetMessageLog(message);
          db.MessageLog.Add(msgLog);
          db.SaveChanges();

          string plainMessage = message.plain;
          var locationMatch = Regex.Match(plainMessage, "(4\\d\\.\\d+)[N ,]+[W\\- ]?(12\\d+\\.\\d+)", RegexOptions.IgnoreCase);
          if (locationMatch.Success)
          {
            plainMessage += string.Format(" http://maps.google.com/?q={0},-{1}", locationMatch.Groups[1].Value, locationMatch.Groups[2].Value);
          }

          // A quick in-memory duplicate check backed up by a database dupe check in case we've been recycled.
          DateTime duplicateTime = DateTime.UtcNow.AddMinutes(-5);
          lock (cacheLock)
          {
            if (messageCache.Contains(plainMessage))
            {
              return "Duplicate";
            }
            else
            {
              messageCache.Add(plainMessage, DateTime.UtcNow, DateTimeOffset.UtcNow.AddMinutes(5));
            }
          }

          if (db.InvoiceItems.Any(f => f.Message.Text == plainMessage && f.SendTime > duplicateTime))
          {
            return "Duplicate";
          }

          var list = db.Phones.Where(f => f.Active).ToList();
          if (TwilioProvider.HasTwilio())
          {
            var twilioClient = TwilioProvider.GetTwilio();
            var twilioFroms = TwilioProvider.GetNumbers();
            var fromIndex = 0;
            var twilioCallback = ConfigurationManager.AppSettings["twilio:callback"];

            foreach (var item in list)
            {
              var twilioMsg = twilioClient.SendMessage(twilioFroms[fromIndex], item.Address, plainMessage, twilioCallback);
              fromIndex = (fromIndex + 1) % twilioFroms.Length;
              db.InvoiceItems.Add(new InvoiceLog { SendTo = item, Sid = twilioMsg.Sid, SendTime = DateTime.UtcNow, Message = msgLog });
              db.SaveChanges();
            }
          }
        }
      }
      catch (Exception ex)
      {
        // debugger here:
        throw;
      }
      return "OK";
    }

    [Route("api/smsstatus")]
    public object MessageCallback(TwilioCallback data)
    {
      if (data.MessageStatus == "delivered" && TwilioProvider.HasTwilio())
      {
        using (var db = new Email2SmsContext())
        {
          var twilio = TwilioProvider.GetTwilio();
          var msg = twilio.GetMessage(data.MessageSid);

          using (var scope = db.Database.BeginTransaction())
          {
            try
            {
              var dbMsg = db.InvoiceItems
                .Where(f => f.Sid == data.MessageSid)
                .Select(f => new { Msg = f, Sub = f.SendTo.Subscription }).FirstOrDefault();

              if (dbMsg != null && !dbMsg.Msg.Price.HasValue)
              {
                dbMsg.Msg.Price = msg.Price;
                db.SaveChanges();
                scope.Commit();
              }
            }
            catch (Exception)
            {
              scope.Rollback();
              throw;
            }
          }
        }
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
