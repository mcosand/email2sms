using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace email2sms.Data
{
  public class Phone
  {
    public int Id { get; set; }
    public string Address { get; set; }
    public virtual Subscription Subscription { get; set; }
  }
}