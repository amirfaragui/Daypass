using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ValueCards.Models;

namespace ValueCards.Services
{
  public interface IApiClient
  {
    void SetCredential(Credential credential);
    IObservable<ConsumerDetail> GetConsumerDetails(int? contractId, CancellationToken cancellationToken = default);
    IAsyncEnumerable<ConsumerDetail> GetConsumerDetailsAsync(int? contractId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Cashier>> GetCashiersAsync(CancellationToken cancellationToken = default);
    Task<Shift> GetActiveShiftAsync(string cashierContractId, string cashierConsumerId, CancellationToken cancellationToken = default);
    Task<Shift> CreateShiftAsync(Cashier cashier, Device device, CancellationToken cancellationToken = default);
    Task<IEnumerable<Device>> GetDevicesAsync(CancellationToken cancellationToken = default);
  }
}
