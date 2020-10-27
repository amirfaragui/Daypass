using Kendo.Mvc.UI;

namespace ValueCards.Services
{
  public interface IConsumerService
  {
    DataSourceResult Read([DataSourceRequest] DataSourceRequest request);
  }
}
