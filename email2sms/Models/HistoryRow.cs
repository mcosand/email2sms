using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace email2sms.Models
{
  public class HistoryRow
  {
    public string Phone { get; set; }
    public DateTime When { get; set; }
    public string Message { get; set; }
    public decimal? Cost { get; set; }
  }
}