using DayPass.Data;
using Kendo.Mvc.UI;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using ValueCards.Models;

namespace ValueCards.Services
{
  public class CachedConsumerRepository : IConsumerRepository
  {
    private readonly ILogger<CachedConsumerRepository> _logger;
    private readonly IApiClient _apiClient;
    private readonly IMemoryCache _cache;
    private readonly WebServiceOption _options;
    private readonly ApplicationDBContext _dbContext;

    private readonly object _syncLock;

    public CachedConsumerRepository(ILogger<CachedConsumerRepository> logger,
                                    IOptions<WebServiceOption> options,
                                    IApiClient apiClient,
                                    IMemoryCache cache,
                                    ApplicationDBContext dbContext)
    {
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
      _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
      _cache = cache ?? throw new ArgumentNullException(nameof(cache));
      _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
      _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
      _syncLock = new object();
    }

    public IEnumerable<CONGBARCODE> Consumers
    {
      get
      {
        var consumers = _cache.Get("consumers") as List<CONGBARCODE>;
        if (consumers == null)
        {
          lock (_syncLock)
          {
           consumers = _dbContext.CONGBARCODE.ToEnumerable().ToList();
            _cache.Set("consumers", consumers, TimeSpan.FromMinutes(10));
          }
        }
        return consumers;
      }
    }

    public IEnumerable<CONGBARCODE> Read(DataSourceRequest request)
    {
            // Use the cache as before.
            var consumers = _cache.Get("consumers") as List<CONGBARCODE>;
            if (consumers != null)
            {
                return consumers;
            }

            lock (_syncLock)
            {
                
                consumers = _cache.Get("consumers") as List<CONGBARCODE>;
                if (consumers != null)
                {
                    return consumers;
                }
           
                var allBarcodes = _dbContext.CONGBARCODE.ToList();
                consumers = allBarcodes.ToList();
                _cache.Set("consumers", consumers, TimeSpan.FromMinutes(10));
            }

            return consumers;
        }

    public void UpdateValue(string epan,  decimal toppedUpAmount)
    {
      lock (_syncLock)
      {
        var consumers = _cache.Get("consumers") as List<CONGBARCODE>;
        if (consumers != null)
        {
          var consumer = consumers.FirstOrDefault(i => i.CEPAN.Contains(epan));
          if (consumer != null)
          {
            consumer.DAYVALD = toppedUpAmount;
            _cache.Set("consumers", consumers, TimeSpan.FromMinutes(10));
          }
        }
      }
    }


        public IEnumerable<ConsumerDetail> APIConsumers
        {
            get
            {
                var consumers = _cache.Get("apiconsumers") as List<ConsumerDetail>;
                if (consumers == null)
                {
                    lock (_syncLock)
                    {
                        consumers = _apiClient.GetConsumerDetails(_options.ContractNumbersOfInterest?.FirstOrDefault()).ToEnumerable().ToList();

                        _cache.Set("apiconsumers", consumers, TimeSpan.FromMinutes(10));
                    }
                }
                return consumers;
            }
        }

        public IEnumerable<ConsumerDetail> APIRead(DataSourceRequest request)
        {
            var consumers = _cache.Get("apiconsumers") as List<ConsumerDetail>;
            if (consumers == null)
            {
                lock (_syncLock)
                {
                    var pageSize = request.PageSize;
                    var count = 0;
                    using var hEvent = new ManualResetEventSlim();

                    consumers = new List<ConsumerDetail>();
                    var o = _apiClient.GetConsumerDetails(_options.ContractNumbersOfInterest?.FirstOrDefault());
                    var v = o.Subscribe(
                      next =>
                      {
                          consumers.Add(next);
                          count++;
                          if (count >= pageSize)
                          {
                              hEvent.Set();
                          }
                      },
                      error =>
                      {
                          throw error;
                      },
                      () =>
                      {
                          hEvent.Set();
                      });
                    var d = o.Subscribe(i =>
                    {
                    });

                    hEvent.Wait();

                    _cache.Set("apiconsumers", consumers, TimeSpan.FromMinutes(10));
                }
            }

            return consumers;

        }

        public void APIUpdateCachedValues(string contractId, string consumerId, decimal toppedUpAmount)
        {
            lock (_syncLock)
            {
                var consumers = _cache.Get("apiconsumers") as List<ConsumerDetail>;
                if (consumers != null)
                {
                    var consumer = consumers.FirstOrDefault(i => i.Consumer.ContractId == contractId && i.Consumer.Id == consumerId);
                    if (consumer != null)
                    {
                        consumer.Balance += toppedUpAmount;
                        _cache.Set("apiconsumers", consumers, TimeSpan.FromMinutes(10));
                    }
                }
            }
        }
    }
}
