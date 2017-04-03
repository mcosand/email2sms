using System.Configuration;
using Twilio;

namespace email2sms
{
  public static class TwilioProvider
  {
    public static bool HasTwilio()
    {
      return !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["twilio:sid"])
        && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["twilio:token"])
        && !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["twilio:numbers"]);
    }

    public static TwilioRestClient GetTwilio()
    {
      var twilioSid = ConfigurationManager.AppSettings["twilio:sid"];
      var twilioToken = ConfigurationManager.AppSettings["twilio:token"];

      return new TwilioRestClient(twilioSid, twilioToken);
    }

    public static string[] GetNumbers()
    {
      return ConfigurationManager.AppSettings["twilio:numbers"].Split(',');
    }
  }
}