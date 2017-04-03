using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace email2sms.Data
{
  public class ErrorRow
  {
    public int Id { get; set; }
    public DateTime TimeUtc { get; set; }
    public string User { get; set; }
    public string Message { get; set; }
    public string Stack { get; set; }
  }
}