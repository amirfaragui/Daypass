using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ValueCards.Models;

namespace ValueCards.Services
{
  public class ConsumerService : IConsumerService
  {
    private readonly ILogger<ConsumerService> _logger;
    private readonly IApiClient _apiClient;
    private readonly IConsumerRepository _repository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ConsumerService(ILogger<ConsumerService> logger,
                           IApiClient apiClient,
                           IHttpContextAccessor httpContextAccessor,
                           IConsumerRepository repository)
    {
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
      _repository = repository ?? throw new ArgumentNullException(nameof(repository));
      _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
      _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    public DataSourceResult APIRead([DataSourceRequest] DataSourceRequest request)
    {
      return _repository.APIRead(request).ToDataSourceResult(request, i => new ApiConsumerModel
      {
        Id = $"{i.Consumer.ContractId},{i.Consumer.Id}",
        FirstName = i.Person?.FirstName ?? i.FirstName,
        Surname = i.Person?.Surname ?? i.Surname,
        ValidUntil = i.Consumer.ValidUntil,
        CardNumber = i.CardNumber, // i.Identification.CardNumber ?? $"{i.Consumer.ContractId},{i.Consumer.Id}",
        Balance= i.Balance,
      });
    }

    public DataSourceResult Read([DataSourceRequest] DataSourceRequest request)
    {
            return _repository.Read(request).ToDataSourceResult(request, i => new DayPassModel
            {
                Id = i.CTRACK,
                CEPAN = i.CEPAN,
                StartDate = i.DTCREAT,
                EndDate = i.DTEXPIRE,
                NumberofDaysPurchased = i.MAXEXIT.ToString(),
                NumberofDaysRemaining = i.DAYVALD.ToString(),
            });
    }

    public async Task<Transaction> PostPaymentAsync(APIConsumerTopupModel model, CancellationToken cancellationToken = default)
    {
      var user = _httpContextAccessor.HttpContext.User;

      var parts = model.Id.Split(',');

      var t = new Transaction()
      {
        ShiftId = user.FindFirst("shiftId").Value,
        ComputerId = user.FindFirst("computerId").Value,
        DeviceId = user.FindFirst("deviceId").Value,
        CashierContractId = user.FindFirst("cashierContractId").Value,
        CashierConsumerId = user.FindFirst("cashierConsumerId").Value,
      };
      var c = new MoneyValueCard(model.Amount)
      {
        ContractId = parts[0],
        ConsumerId = parts[1],
      };
      var p = new TransactionDetail(c)
      {
        Transaction = t
      };

      var result = await _apiClient.PostPayment(p);

      _repository.APIUpdateCachedValues(parts[0], parts[1], model.Amount);      

      return result;
    }

  }
}
