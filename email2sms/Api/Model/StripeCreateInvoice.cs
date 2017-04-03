using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace email2sms.Api.Model
{
  public class StripeCreateInvoice
  {
    public string Id { get; set; }
    public string Customer { get; set; }
    public long Period_Start { get; set; }
    public long Period_End { get; set; }
  }
}