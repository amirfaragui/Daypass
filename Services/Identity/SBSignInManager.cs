using ValueCards.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Claims;
using System.Linq;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace ValueCards.Services.Identity
{
  public class SBSignInManager : SignInManager<SBUser>
  {
    private readonly IApiClient _apiClient;

    public SBSignInManager(
      UserManager<SBUser> userManager,
              IHttpContextAccessor contextAccessor,
              IUserClaimsPrincipalFactory<SBUser> claimsFactory,
              IOptions<IdentityOptions> optionsAccessor,
              ILogger<SignInManager<SBUser>> logger,
              IAuthenticationSchemeProvider schemeProvider,
              IUserConfirmation<SBUser> userConfirmation,
              IApiClient apiClient)
      : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemeProvider, userConfirmation)
    {
      _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
    }

    public override async Task<SignInResult> CheckPasswordSignInAsync(SBUser user, string password, bool lockoutOnFailure)
    {
      _apiClient.SetCredential(new Credential
      {
        Username = user.UserName,
        Password = password,
      });

      try
      {
        var cashiers = await _apiClient.GetCashiersAsync();
        var cashier = cashiers.FirstOrDefault(i => i.CashierConsumerId == user.UserName);
        if (cashier == null)
        {
          return SignInResult.Failed;
        }
        user.Password = password;
        user.CashierConsumerId = cashier.CashierConsumerId;
        user.CashierContractId = cashier.CashierContractId;

        await UserManager.UpdateAsync(user);

      }
      catch (Exception ex)
      {
        Logger.LogError(ex.ToString());
        return SignInResult.Failed;
      }

      return SignInResult.Success;
    }

    public override async Task SignInWithClaimsAsync(SBUser user, AuthenticationProperties authenticationProperties, IEnumerable<Claim> additionalClaims)
    {
      var userPrincipal = await CreateUserPrincipalAsync(user);
      foreach (var claim in additionalClaims)
      {
        userPrincipal.Identities.First().AddClaim(claim);
      }
      await Context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
          userPrincipal,
          authenticationProperties ?? new AuthenticationProperties());
    }

  }
}
