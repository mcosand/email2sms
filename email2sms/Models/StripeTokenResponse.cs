using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace email2sms.Models
{
  public class StripeTokenResponse
  {
    public string StripeToken { get; set; }
    public string StripeTokenType { get; set; }
    public string StripeEmail { get; set; }
  }
}