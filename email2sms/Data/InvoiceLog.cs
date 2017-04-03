using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace email2sms.Data
{
  public class InvoiceLog
  {
    public int Id { get; set; }
    [Column("Phone_Id")]
    public int PhoneId { get; set; }
    [ForeignKey("PhoneId")]
    public virtual Phone SendTo { get; set; }
    [Column("Message_Id")]
    public int MessageId { get; set; }
    [ForeignKey("MessageId")]
    public virtual MessageLog Message { get; set; }
    public DateTime SendTime { get; set; }
    public decimal? Price { get; set; }
    public string Sid { get; set; }
  }
}