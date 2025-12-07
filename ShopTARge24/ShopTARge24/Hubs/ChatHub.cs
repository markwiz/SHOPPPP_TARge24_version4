using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace ShopTARge24.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            var time = DateTime.Now.ToString("HH:mm:ss");


            await Clients.All.SendAsync("ReceiveMessage", user, message, time);
        }
    }
}
