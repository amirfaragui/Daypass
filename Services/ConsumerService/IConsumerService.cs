using Kendo.Mvc.UI;
using System.Threading;
using System.Threading.Tasks;
using ValueCards.Models;

namespace ValueCards.Services
{
  public interface IConsumerService
  {
    DataSourceResult Read([DataSourceRequest] DataSourceRequest request);

    Task<Transaction> PostPaymentAsync(ConsumerTopupModel model, CancellationToken cancellationToken = default);
  }
}
