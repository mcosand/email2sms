using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace email2sms.Data
{
  public class Email2SmsContext : DbContext
  {
    public Email2SmsContext() : base("DefaultConnection") {
    }

    public IDbSet<Phone> Phones { get; set; }
    public IDbSet<Subscription> Subscriptions { get; set; }
    public IDbSet<MessageLog> MessageLog { get; set; }
    public IDbSet<InvoiceLog> InvoiceItems { get; set; }

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
      modelBuilder.Types().Configure(e => e.ToTable("email2sms_" + e.ClrType.Name));
      base.OnModelCreating(modelBuilder);
      modelBuilder.Properties<decimal>().Configure(f => f.HasPrecision(18,4));
    }
  }

}