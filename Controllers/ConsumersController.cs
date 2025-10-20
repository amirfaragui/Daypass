using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using ValueCards.Models;
using ValueCards.Services;

namespace ValueCards.Controllers
{
  [Authorize(Policy = CookieAuthenticationDefaults.AuthenticationScheme)]
  public class ConsumersController : Controller
  {
    private readonly IConsumerService _consumerService;
    private readonly ILogger<ConsumersController> _logger;
    private readonly IApiClient _apiClient;
    public ConsumersController(IConsumerService consumerService,
                               
                               ILogger<ConsumersController> logger)
    {
      _consumerService = consumerService ?? throw new ArgumentNullException(nameof(consumerService));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public IActionResult Index()
    {
      return View();
    }

    public async Task<IActionResult> Read([DataSourceRequest]DataSourceRequest request)
    {
      return Json(_consumerService.Read(request));
    }

    public IActionResult Topup(string id, [FromServices] IConsumerRepository repository)
    {
      if (id == null)
        throw new ArgumentNullException(nameof(id));

   /**   var parts = id.Split(',');
      if (parts.Length != 2)
        return BadRequest();**/

      var item = repository.Consumers
        .Where(i => i.CEPAN.Contains(id))
        .Select(i => new ConsumerTopupModel
        {
          Id = i.CTRACK,
          CEPAN = i.CEPAN,
          StartDate = i.DTCREAT.ToString(),
          EndDate = i.DTEXPIRE.ToString(),
          Amount = i.MAXEXIT,
        })
        .FirstOrDefault();

      if (item == null)
        return NotFound();

      return View(item);
    }

    [HttpPost]
    public async Task<IActionResult> Topup(string id, APIConsumerTopupModel model)
    {
      if(model == null)
        throw new ArgumentNullException(nameof(model));

      if (string.IsNullOrEmpty(id))
        return BadRequest();

      var parts = id.Split(',');
      if (parts.Length != 2)
        return BadRequest();

      try
      {
        model.Id = id;
        await _consumerService.PostPaymentAsync(model);
        return Created("", model);
      }
      catch (ApiErrorException aex)
      {
        return new ObjectResult(new { message = aex.Message }) { StatusCode = (int)aex.StutusCode };
      }
      catch (Exception ex)
      {
        _logger.LogError(ex.ToString());
        throw;
      }
    }
  }
}
