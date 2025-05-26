using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Core.Application.Website.Utils
{
    public class FormatValidationsHub : Hub
    {
        public async Task Send()
        {
            await Clients.All.SendAsync("receive");
        }
    }
}
