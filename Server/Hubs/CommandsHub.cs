using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace HVMDash.Server.Hubs
{
    public class CommandsHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            //Store 100 msg
            ////get last 100 messages from storage
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
