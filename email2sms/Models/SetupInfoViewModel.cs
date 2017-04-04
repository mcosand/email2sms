using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using email2sms.Data;

namespace email2sms.Models
{
  public class SetupInfoViewModel
  {
    public Subscription Customer { get; set; }
    public string[] Phones { get; set; } = new string[0];
    public string[] Inactive { get; set; } = new string[0];
    public string Email { get; set; }
  }
}