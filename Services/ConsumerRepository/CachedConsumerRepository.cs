using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
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

    private readonly object _syncLock;

    public CachedConsumerRepository(ILogger<CachedConsumerRepository> logger,
                                    IOptions<WebServiceOption> options,
                                    IApiClient apiClient,
                                    IMemoryCache cache)
    {
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
      _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
      _cache = cache ?? throw new ArgumentNullException(nameof(cache));
      _options = options?.Value ?? throw new ArgumentNullException(nameof(options));

      _syncLock = new object();
    }

    public IEnumerable<ConsumerDetail> Consumers
    {
      get
      {
        var consumers = _cache.Get("consumers") as List<ConsumerDetail>;
        if (consumers == null)
        {
          lock (_syncLock)
          {
            consumers = _apiClient.GetConsumerDetails(_options.ContractNumbersOfInterest?.FirstOrDefault()).ToEnumerable().ToList();

            _cache.Set("consumers", consumers, TimeSpan.FromMinutes(10));
          }
        }
        return consumers;
      }
    }

    public IEnumerable<ConsumerDetail> Read(DataSourceRequest request)
    {
      var consumers = _cache.Get("consumers") as List<ConsumerDetail>;
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

          _cache.Set("consumers", consumers, TimeSpan.FromMinutes(10));
        }
      }

      return consumers;

    }


  }
}
