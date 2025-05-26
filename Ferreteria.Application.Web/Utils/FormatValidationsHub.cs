using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Ferreteria.Application.Website.Utils
{
    public class FormatValidationsHub : Hub
    {
        public async Task Send()
        {
            await Clients.All.SendAsync("receive");
        }
    }
}
