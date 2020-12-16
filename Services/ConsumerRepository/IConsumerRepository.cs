using Kendo.Mvc.UI;
using System.Collections.Generic;
using ValueCards.Models;

namespace ValueCards.Services
{
  public interface IConsumerRepository
  {
    IEnumerable<ConsumerDetail> Consumers { get; }

    IEnumerable<ConsumerDetail> Read(DataSourceRequest request);

    void UpdateCachedValues(string contractId, string consumerId, decimal toppedUpAmount);
  }
}
