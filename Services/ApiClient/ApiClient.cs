using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serializers;
using RestSharp.Serializers.NewtonsoftJson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Xml;
using System.Xml.Serialization;
using ValueCards.Models;

namespace ValueCards.Services
{
  public class ApiClient : IApiClient
  {
    private readonly WebServiceOption _serviceOption;
    private readonly ILogger<ApiClient> _logger;

    private IRestClient _client;

    public ApiClient(ILogger<ApiClient> logger,
                     IOptions<WebServiceOption> options)
    {
      _serviceOption = options?.Value ?? throw new ArgumentNullException(nameof(options));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));

      _client = new RestClient(_serviceOption.Url);
      _client.Authenticator = new HttpBasicAuthenticator(_serviceOption.Credential.Username, _serviceOption.Credential.Password);
      _client.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
      _client.UseNewtonsoftJson();
      _client.FailOnDeserializationError = false;
    }

    public void SetCredential(Credential credential)
    {
      _client.Authenticator = new HttpBasicAuthenticator(credential.Username, credential.Password);
    }

    #region Consumers
    public IObservable<ConsumerDetail> GetConsumerDetails(int? contractId, CancellationToken cancellationToken = default)
    {
      var subject = new Subject<ConsumerDetail>();
      Task.Run(() => GetConsumerDetailsTask(contractId, subject, cancellationToken));
      return subject;
    }

    public IAsyncEnumerable<ConsumerDetail> GetConsumerDetailsAsync(int? contractId, CancellationToken cancellationToken = default)
    {
      return GetConsumerDetails(contractId, cancellationToken).ToAsyncEnumerable();
    }

    private async IAsyncEnumerable<Consumer> GetConsumers(int? contractId, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
      var request = new RestRequest("CustomerMediaWebService/consumers");
      if (contractId.HasValue)
      {
        var cid = contractId.Value.ToString();
        request.AddQueryParameter("minContractId", cid);
        request.AddQueryParameter("maxContractId", cid);
      }
      request.AddHeader("Accept", "application/json");

      var consumers = await _client.GetAsync<IEnumerable<Consumer>>(request, cancellationToken);

      foreach (var c in consumers)
      {
        yield return c;
      }
    }

    private async Task GetConsumerDetailsTask(int? contractId, Subject<ConsumerDetail> subject, CancellationToken cancellationToken)
    {
      try
      {
        var request = new RestRequest("CustomerMediaWebService/consumers");
        if (contractId.HasValue)
        {
          var cid = contractId.Value.ToString();
          request.AddQueryParameter("minContractId", cid);
          request.AddQueryParameter("maxContractId", cid);
        }
        request.AddHeader("Accept", "application/json");
        request.Method = Method.GET;

        var getBlock = new ActionBlock<Consumer>(async c =>
        {
          try
          {
            var detailRequest = new RestRequest(@$"CustomerMediaWebService/consumers/{c.ContractId},{c.Id}/detail");
            detailRequest.AddHeader("Accept", "application/json");
            detailRequest.Method = Method.GET;

            var response = await _client.ExecuteGetAsync<ConsumerDetailResponse>(detailRequest, cancellationToken);
            _logger.LogDebug(response.Content);
            var details = response.Data.ConsumerDetail;
            if (details?.Identification != null)
            {
              if (details.Identification.PtcptType == 6)   // Value Card
              {
                try
                {
                  var balanceRequest = new RestRequest($@"PaymentWebService/media/personalizedMoneyValue/{c.ContractId},{c.Id}");
                  var balanceResponse = await _client.ExecuteGetAsync<BalanceResponse>(balanceRequest, cancellationToken);
               
                  if(balanceResponse.Data.MoneyValue.HasValue)
                  {
                    details.Balance = balanceResponse.Data.MoneyValue.Value / 100;
                  }
                }
                catch (Exception ex)
                {
                  _logger.LogError(ex.ToString());
                }

                subject.OnNext(details);
              }
            }
          }
          catch (Exception ex)
          {
            _logger.LogError(ex.ToString());
          }
        }, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 3 });

        IEnumerable<Consumer> consumers = null;
        var response  = await _client.ExecuteGetAsync(request, cancellationToken);
        _logger.LogDebug(response.Content);
        try
        {
          var result = JsonConvert.DeserializeObject<ConsumerListResponse>(response.Content);
          consumers = result.Consumers.Consumers;
        }
        catch (JsonSerializationException)
        {
          var result = JsonConvert.DeserializeObject<SingleConsumer>(response.Content);
          consumers = result.Consumers;
        }
        foreach (var c in consumers)
        {
          getBlock.Post(c);
        }
        getBlock.Complete();
        await getBlock.Completion;
        subject.OnCompleted();
      }
      catch (Exception ex)
      {
        _logger.LogError(ex.ToString());
        subject.OnError(ex);
        throw;
      }
    }
    #endregion

    #region Cashier
    public async Task<IEnumerable<Cashier>> GetCashiersAsync(CancellationToken cancellationToken = default)
    {
      try
      {
        var request = new RestRequest("PaymentWebService/cashiers");
        request.AddHeader("Accept", "application/json");
        request.Method = Method.GET;
        var response = await _client.ExecuteGetAsync(request, cancellationToken);
        switch(response.StatusCode)
        {
          case System.Net.HttpStatusCode.Unauthorized:
            throw new UnauthorizedAccessException();
        }

        if(response.IsSuccessful)
        {
          var result = JsonConvert.DeserializeObject<CashierListResponse>(response.Content);
          return result.Cashiers;
        }

        throw new HttpRequestException() { HResult = (int)response.StatusCode };        
      }
      catch (Exception ex)
      {
        _logger.LogError(ex.ToString());
        throw;
      }
    }

    #endregion

    #region Shift
    public async Task<Shift> GetActiveShiftAsync(string cashierContractId, string cashierConsumerId, CancellationToken cancellationToken = default)
    {
      try
      {
        var request = new RestRequest($"PaymentWebService/cashiers/{cashierContractId},{cashierConsumerId}/shifts");
        request.AddHeader("Accept", "application/json");
        request.Method = Method.GET;
        var response = await _client.ExecuteGetAsync(request, cancellationToken);

        if (response.IsSuccessful)
        {
          if(response.Content == "null")
          {
            return null;
          }

          try
          {
            var result = JsonConvert.DeserializeObject<ShiftResponse>(response.Content);
            var shift = result.Shift;
            if(shift.FinishDateTime.HasValue)
            {
              return null;
            }
            return shift;
          }
          catch (JsonSerializationException)
          {
            var result = JsonConvert.DeserializeObject<ShiftListResponse>(response.Content);
            var shift = result.Shifts.FirstOrDefault(i => i.ShiftStatus == 1);
            return shift;
          }

        }

        throw new HttpRequestException() { HResult = (int)response.StatusCode };
      }
      catch (Exception ex)
      {
        _logger.LogError(ex.ToString());
        throw;
      }
    }

    public async Task<Shift> CreateShiftAsync(Cashier cashier, Device device, CancellationToken cancellationToken = default)
    {
      try
      {
        var request = new RestRequest($"PaymentWebService/shifts");
        request.AddHeader("Accept", "application/json");        
        request.Method = Method.POST;
        request.XmlSerializer = new DotNetXmlSerializer("http://gsph.sub.com/payment/types");

        var body = new NewShift()
        {
          CashierContractId = cashier.CashierContractId,
          CashierConsumerId = cashier.CashierConsumerId,
          ComputerId = device.ComputerId,
          DeviceId = device.DeviceId,
          ShiftNo = DateTime.Now.ToString("yyMMddHHmm"),
          CreateDateTime = DateTime.Now,
        };
        
        request.AddXmlBody(body, "http://gsph.sub.com/payment/types");

        var response = await _client.ExecutePostAsync(request, cancellationToken);
        _logger.LogDebug(response.Content);

        if (response.IsSuccessful)
        {
          var shift = JsonConvert.DeserializeObject<Shift>(response.Content);
          return shift;
        }

        try
        {
          var error = JsonConvert.DeserializeObject<ErrorResponse>(response.Content);
          throw new ApiErrorException(error.Error) { StutusCode = response.StatusCode };
        }
        catch (JsonSerializationException)
        {
          throw new HttpRequestException() { HResult = (int)response.StatusCode };
        }
      }
      catch (Exception ex)
      {
        _logger.LogError(ex.ToString());
        throw;
      }
    }
    #endregion

    #region Device
    public async Task<IEnumerable<Device>> GetDevicesAsync(CancellationToken cancellationToken = default)
    {
      try
      {
        var request = new RestRequest($"PaymentWebService/devices");
        request.AddHeader("Accept", "application/json");
        request.Method = Method.GET;
        var response = await _client.GetAsync<DeviceListResponse>(request, cancellationToken);
        return response.Devices;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex.ToString());
        throw;
      }
    }
    #endregion

    public async Task<Transaction> PostPayment(TransactionDetail transaction, CancellationToken cancellationToken = default)
    {
      try
      {
        var request = new RestRequest($"PaymentWebService/shifts/{transaction.Transaction.ShiftId}/salestransactions");
        request.AddHeader("Accept", "application/json");
        request.AddHeader("Content-Type", "application/xml");
        request.Method = Method.PUT;
        //request.XmlSerializer = new DotNetXmlSerializer("http://gsph.sub.com/payment/types");
        //request.AddXmlBody(transaction, "http://gsph.sub.com/payment/types");

        using var stream = new MemoryStream();
        using var writer = new XmlTextWriter(stream, Encoding.UTF8);
        var serializer = new System.Xml.Serialization.XmlSerializer(typeof(TransactionDetail));

        var ns = new XmlSerializerNamespaces();
        ns.Add("pay", "http://gsph.sub.com/payment/types");

        serializer.Serialize(writer, transaction, ns);

        var body = Encoding.UTF8.GetString(stream.ToArray());
        _logger.LogDebug(body);

        request.AddParameter("application/xml", body, ParameterType.RequestBody);

        var response = await _client.ExecuteAsync(request, cancellationToken);
        _logger.LogDebug(body);

        if (response.IsSuccessful)
        {
          var result = JsonConvert.DeserializeObject<Transaction>(response.Content);
          return result;
        }

        try
        {
          var error = JsonConvert.DeserializeObject<ErrorResponse>(response.Content);
          throw new ApiErrorException(error.Error) { StutusCode = response.StatusCode };
        }
        catch (JsonSerializationException)
        {
          throw new HttpRequestException() { HResult = (int)response.StatusCode };
        }
      }
      catch (Exception ex)
      {
        _logger.LogError(ex.ToString());
        throw;
      }

    }
  }
}
