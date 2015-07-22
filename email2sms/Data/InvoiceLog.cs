using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace email2sms.Data
{
  public class InvoiceLog
  {
    public int Id { get; set; }
    public virtual Phone SendTo { get; set; }
    public virtual MessageLog Message { get; set; }
    public DateTime SendTime { get; set; }
    public decimal? Price { get; set; }
    public string Sid { get; set; }
  }
}