using ValueCards.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace ValueCards.Services.Identity
{
  public class SBSignInManager : SignInManager<SBUser>
  {
    private readonly UserManager<SBUser> _userManager;
    private readonly IHttpContextAccessor _contextAccessor;
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
      _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
      _contextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(contextAccessor));
      _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
    }

    public override Task<SignInResult> CheckPasswordSignInAsync(SBUser user, string password, bool lockoutOnFailure)
    {

      return base.CheckPasswordSignInAsync(user, password, lockoutOnFailure);
    }
  }
}
