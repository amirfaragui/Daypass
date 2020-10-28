using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serializers.NewtonsoftJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
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

            var response = await _client.GetAsync<ConsumerDetailResponse>(detailRequest, cancellationToken);
            var details = response?.ConsumerDetail;
            if (details?.Identification != null)
            {
              if (details.Identification.PtcptType == 6)   // Value Card
              {
                subject.OnNext(details);
              }
            }
          }
          catch (Exception ex)
          {
            _logger.LogError(ex.ToString());
          }
        }, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 3 });

        var result = await _client.GetAsync<ConsumerListResponse>(request, cancellationToken);
        foreach (var c in result.Consumers.Consumers)
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
  }
}
