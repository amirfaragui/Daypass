using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ValueCards.Hubs
{
  public class TenantHub : Hub<IViewHub>
  {
    //public async Task ViewUpdateSuggested(string connectionId)
    //{
    //  await Clients.Client(connectionId).ViewUpdateSuggested(connectionId);
    //}

    public override async Task OnConnectedAsync()
    {
      await base.OnConnectedAsync();
      await Clients.Caller.ConnectionEstablished(Context.ConnectionId); 
    }
  }
}
