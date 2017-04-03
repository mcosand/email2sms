using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace email2sms.Api.Model
{
  public class StripeMessage<T>
  {
    public StripeObject<T> Data { get; set; }
    public string Type { get; set; }
  }
}