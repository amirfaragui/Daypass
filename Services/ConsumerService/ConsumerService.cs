using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ValueCards.Models;

namespace ValueCards.Services
{
  public class ConsumerService: IConsumerService
  {
    private readonly ILogger<ConsumerService> _logger;
    private readonly WebServiceOption _webService;

    public ConsumerService(IOptions<WebServiceOption> options,
                           ILogger<ConsumerService> logger)
    {
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
      _webService = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

  }
}
