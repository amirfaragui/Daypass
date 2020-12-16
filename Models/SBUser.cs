using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ValueCards.Models
{
  public class SBUser: IdentityUser<string>
  {
    public string Password { get; set; }
    public string CashierContractId { get; set; }
    public string CashierConsumerId { get; set; }
    public string ShiftId { get; set; }
    public string ComputerId { get; set; }
    public string DeviceId { get; set; }
  }
}
