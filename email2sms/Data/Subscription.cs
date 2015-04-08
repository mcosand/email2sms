using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace email2sms.Data
{
  public class Subscription
  {
    public int Id { get; set; }
    public string Email { get; set; }
    public decimal Balance { get; set; }
    public virtual ICollection<Phone> Phones { get; set; }
  }
}