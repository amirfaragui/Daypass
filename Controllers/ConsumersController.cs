using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ValueCards.Services;

namespace ValueCards.Controllers
{
  public class ConsumersController : Controller
  {
    private readonly IConsumerService _consumerService;
    private readonly ILogger<ConsumersController> _logger;
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
  }
}
