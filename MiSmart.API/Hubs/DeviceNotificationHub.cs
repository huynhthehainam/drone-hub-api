

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Connections.Features;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

using MiSmart.DAL.DatabaseContexts;
using MiSmart.DAL.Repositories;

namespace MiSmart.API.Hubs
{
    public class DeviceWebSocketClient
    {
        public String ConnectionID { get; set; }
        public Int32 DeviceID { get; set; }
    }
    [Authorize]
    public class DeviceNotificationHub : Hub
    {
        private readonly IServiceProvider serviceProvider;

        public DeviceNotificationHub(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
        }


        public async Task PublicUpdateEventAsync(Int32 id, String attributeName, Object data)
        {
            await Clients.All.SendAsync("ReceiveUpdate", JsonSerializer.Serialize(new { ID = id, Attribute = attributeName, Data = data }, JsonOptions.CamelOptions));
        }
    }
}