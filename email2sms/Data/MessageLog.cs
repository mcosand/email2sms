using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace email2sms.Data
{
  public class MessageLog
  {
    public int Id { get; set; }
    public string Json { get; set; }
    public DateTime Received { get; set; }
  }
}