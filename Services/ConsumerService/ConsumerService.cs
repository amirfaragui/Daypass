using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using ValueCards.Models;

namespace ValueCards.Services
{
  public class ConsumerService : IConsumerService
  {
    private readonly ILogger<ConsumerService> _logger;
    private readonly IConsumerRepository _repository;

    public ConsumerService(ILogger<ConsumerService> logger,
                           IConsumerRepository repository)
    {
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
      _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public DataSourceResult Read([DataSourceRequest] DataSourceRequest request)
    {
      return _repository.Read(request).ToDataSourceResult(request, i => new ConsumerModel
      {
        Id = $"{i.Consumer.ContractId},{i.Consumer.Id}",
        FirstName = i.Person?.FirstName ?? i.FirstName,
        Surname = i.Person?.Surname ?? i.Surname,
        ValidUntil = i.Consumer.ValidUntil,
        CardNumber = i.Identification.CardNumber ?? $"{i.Consumer.ContractId},{i.Consumer.Id}",
      });
    }

  }
}
