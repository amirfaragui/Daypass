using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ValueCards.Models;

namespace ValueCards.Services.Identity
{
  public class SBUserClaimsPrincipalFactory : IUserClaimsPrincipalFactory<SBUser>
  {
    public async Task<ClaimsPrincipal> CreateAsync(SBUser user)
    {
      var claims = new List<Claim>();
      claims.Add(new Claim(ClaimTypes.Name, user.UserName));
      claims.Add(new Claim("cashierContractId", user.CashierContractId));
      claims.Add(new Claim("cashierConsumerId", user.CashierConsumerId));
      claims.Add(new Claim("computerId", user.ComputerId));
      claims.Add(new Claim("deviceId", user.DeviceId));
      claims.Add(new Claim("shiftId", user.ShiftId));

      var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
      return new ClaimsPrincipal(identity);
    }
  }
}
