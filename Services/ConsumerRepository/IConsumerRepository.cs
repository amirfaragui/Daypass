using DayPass.Data;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using ValueCards.Models;

namespace ValueCards.Services
{
  public interface IConsumerRepository
  {
    IEnumerable<CONGBARCODE> Consumers { get; }

    IEnumerable<CONGBARCODE> Read(DataSourceRequest request);

    void UpdateValue(string epan, decimal toppedUpAmount, DateTime? endDate);
    IEnumerable<ConsumerDetail> APIConsumers { get; }

   IEnumerable<ConsumerDetail> APIRead(DataSourceRequest request);

   void APIUpdateCachedValues(string contractId, string consumerId, decimal toppedUpAmount);
    }
}
