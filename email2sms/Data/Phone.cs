using System.ComponentModel.DataAnnotations.Schema;

namespace email2sms.Data
{
  public class Phone
  {
    public int Id { get; set; }
    public string Address { get; set; }
    public bool Active { get; set; }

    [Column("Subscription_Id")]
    public int SubscriptionId { get; set; }

    [ForeignKey("SubscriptionId")]
    public virtual Subscription Subscription { get; set; }
  }
}