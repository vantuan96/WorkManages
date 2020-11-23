using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Kztek_Web.SignalR
{
    public class WorkHub: Hub
    {
        public async Task SendMessage()
        {
            await Clients.All.SendAsync("ReceiveMessage", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), Context.ConnectionId);
        }
    }
}