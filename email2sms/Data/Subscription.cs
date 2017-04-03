using System;
using System.Collections.Generic;

namespace email2sms.Data
{
  public class Subscription
  {
    public int Id { get; set; }
    public Guid User { get; set; }
    public string StripeCustomer { get; set; }
    public DateTime? LastInvoiceUtc { get; set; }
    public virtual ICollection<Phone> Phones { get; set; }
  }
}