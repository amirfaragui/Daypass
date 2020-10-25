using ValueCards.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace ValueCards.Services
{
  public class ApplicationSignInManager : SignInManager<ApplicationUser>
  {
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _dbContext;
    private readonly IHttpContextAccessor _contextAccessor;

    public ApplicationSignInManager(
      UserManager<ApplicationUser> userManager,
              IHttpContextAccessor contextAccessor,
              IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory,
              IOptions<IdentityOptions> optionsAccessor,
              ILogger<SignInManager<ApplicationUser>> logger,
              ApplicationDbContext dbContext,
              IAuthenticationSchemeProvider schemeProvider,
              IUserConfirmation<ApplicationUser> userConfirmation)
      : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemeProvider, userConfirmation)
    {
      _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
      _contextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(contextAccessor));
      _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public override Task<SignInResult> CheckPasswordSignInAsync(ApplicationUser user, string password, bool lockoutOnFailure)
    {
      if (!user.Active)
      {
        return Task.FromResult(SignInResult.Failed);
      }

      return base.CheckPasswordSignInAsync(user, password, lockoutOnFailure);
    }
  }
}
