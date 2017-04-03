using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace email2sms.Api.Model
{
  public class StripeObject<T>
  {
    public T Object { get; set; }
  }
}