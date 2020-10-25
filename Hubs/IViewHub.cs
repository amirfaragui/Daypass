using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace ValueCards.Hubs
{
  public interface IViewHub
  {
    Task ConnectionEstablished(string connectionId);
    Task ViewUpdateSuggested(string connectionId);
  }
}
