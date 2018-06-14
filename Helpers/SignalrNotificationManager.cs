using Microsoft.AspNetCore.SignalR;
using Mynt.Core.Interfaces;
using MyntUI;
using MyntUI.Hubs;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Mynt.Core.Notifications
{
    public class SignalrNotificationManager : INotificationManager
    {

        public async Task<bool> SendNotification(string message)
        {
            await Globals.GlobalHubMyntTraders.Clients.All.SendAsync("Send", message);
            return true;
        }

        public async Task<bool> SendTemplatedNotification(string template, params object[] parameters)
        {
            var finalMessage = string.Format(template, parameters);
            return await SendNotification(finalMessage);
        }
    }
}
