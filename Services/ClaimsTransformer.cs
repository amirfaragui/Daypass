using ValueCards.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ValueCards.Services
{
  public class ClaimsTransformer: IClaimsTransformation
  {

    public ClaimsTransformer()
    {
    }

    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
      //var existingClaimsIdentity = (ClaimsIdentity)principal.Identity;
      //if (existingClaimsIdentity.AuthenticationType == IdentityConstants.ApplicationScheme)
      //{
      //  var currentUserName = existingClaimsIdentity.Name;
      //  var nameIdentifier = existingClaimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
      //  var userId = new Guid(nameIdentifier.Value);

      //  // Initialize a new list of claims for the new identity
      //  var claims = new List<Claim>(existingClaimsIdentity.Claims);

      //  // Find the user in the DB
      //  // Add as many role claims as they have roles in the DB
      //  var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
      //  if (user != null)
      //  {
      //    if (user.Type == UserType.OPERATOR)
      //    {
      //      claims.Add(new Claim("operator-id", user.OperatorId?.ToString()));
      //    }
      //    else
      //    {
      //      await _dbContext.Entry(user).Reference(u => u.Tenant).LoadAsync();
      //      claims.Add(new Claim("tenant-id", user.TenantId?.ToString()));
      //      claims.Add(new Claim("tenant-qrcode", user.Tenant.QRCodeSeed));
      //      claims.Add(new Claim("tenant-label1", user.Tenant.DataLabel1 ?? ""));
      //      claims.Add(new Claim("tenant-label2", user.Tenant.DataLabel2 ?? ""));
      //      claims.Add(new Claim("tenant-label3", user.Tenant.DataLabel3 ?? ""));
      //      claims.Add(new Claim("tenant-label4", user.Tenant.DataLabel4 ?? ""));
      //      claims.Add(new Claim("tenant-check-in-label", string.IsNullOrEmpty(user.Tenant.CheckInLabel) ? "Check-in" : user.Tenant.CheckInLabel));
      //      claims.Add(new Claim("tenant-check-out-label", string.IsNullOrEmpty(user.Tenant.CheckOutLabel) ? "Check-out" : user.Tenant.CheckOutLabel));
      //      claims.Add(new Claim("tenant-multi-issue", user.Tenant.AllowMultipleIssue.ToString()));
      //      claims.Add(new Claim("tenant-reservation-label", user.Tenant.ReservationLabel ?? ""));
      //    }

      //  }

      //  // Build and return the new principal
      //  var newClaimsIdentity = new ClaimsIdentity(claims, existingClaimsIdentity.AuthenticationType);
      //  return new ClaimsPrincipal(newClaimsIdentity);
      //}

      return principal;
    }
  }
}
