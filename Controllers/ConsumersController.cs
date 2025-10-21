using DayPass.Data;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    private readonly ApplicationDBContext _dbContext;
    private readonly IConsumerRepository _repository;
    public ConsumersController(IConsumerService consumerService,
                               IConsumerRepository repository,
                              ILogger<ConsumersController> logger,
                               ApplicationDBContext dBContext)
    {
      _consumerService = consumerService ?? throw new ArgumentNullException(nameof(consumerService));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
      _dbContext = dBContext ?? throw new ArgumentNullException((nameof(dBContext)));
      _repository = repository ?? throw new ArgumentNullException(nameof(repository));
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

      var item = repository.Consumers
        .Where(i => i.CEPAN.Contains(id))
        .Select(i => new ConsumerTopupModel
        {
          Id = i.CTRACK,
          CEPAN = i.CEPAN,
          StartDate = i.DTCREAT.ToString(),
          EndDate = i.DTEXPIRE.ToString(),
          Amount = i.DAYVALD,
        })
        .FirstOrDefault();

      if (item == null)
        return NotFound();

      return View(item);
    }

    [HttpPost]
    public async Task<IActionResult> Topup(string id, ConsumerTopupModel model)
    {
      if(model == null)
        throw new ArgumentNullException(nameof(model));

      if (string.IsNullOrEmpty(id))
        return BadRequest();


      try
      {
        
        var entityToUpdate =  _dbContext.CONGBARCODE.SingleOrDefault(e=>e.CEPAN.Contains(id));
        if (entityToUpdate == null)
                return BadRequest();
       entityToUpdate.DAYVALD = model.Amount;
       await _dbContext.SaveChangesAsync();
       _repository.UpdateValue(id,model.Amount);
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
