using Microsoft.AspNetCore.SignalR;
using Mynt.Core.Interfaces;
using MyntUI.Hubs;
using System.Threading.Tasks;

namespace Mynt.Core.Notifications
{
  public class SignalrNotificationManager : INotificationManager
  {
    public static IHubContext<HubMyntTraders> hubMyntTraders;

    public async Task<bool> SendNotification(string message)
    {
      await hubMyntTraders.Clients.All.SendAsync(message);
      return true;
    }

    public async Task<bool> SendTemplatedNotification(string template, params object[] parameters)
    {
      var finalMessage = string.Format(template, parameters);
      return await SendNotification(finalMessage);
    }
  }
}
