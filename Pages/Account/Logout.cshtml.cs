using ValueCards.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ValueCards.Areas.Admin.Pages.Account
{
  [AllowAnonymous]
  public class LogoutModel : PageModel
  {
    private readonly SignInManager<SBUser> _signInManager;
    private readonly ILogger<LogoutModel> _logger;

    public LogoutModel(SignInManager<SBUser> signInManager, ILogger<LogoutModel> logger)
    {
      _signInManager = signInManager;
      _logger = logger;
    }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPost(string returnUrl = null)
    {
      await _signInManager.SignOutAsync();
      _logger.LogInformation("User logged out.");
      if (returnUrl != null)
      {
        return LocalRedirect(returnUrl);
      }
      else
      {
        return RedirectToPage();
      }
    }
  }
}
