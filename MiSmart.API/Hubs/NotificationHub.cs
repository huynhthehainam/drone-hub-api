

using System;
using System.Collections.Generic;
using System.Linq;
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
        public List<DeviceWebSocketClient> DeviceClients { get; set; } = new List<DeviceWebSocketClient>();
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
            var removedClients = DeviceClients.Where(ww => ww.ConnectionID == Context.ConnectionId).ToList();
            foreach (var client in removedClients)
            {
                DeviceClients.Remove(client);
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SetupDeviceAsync(Int32 id)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                using (var databaseContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>())
                {
                    var device = databaseContext.Devices.FirstOrDefault(ww => ww.ID == id);
                    if (device is not null)
                    {
                        DeviceClients.Add(new DeviceWebSocketClient
                        {
                            ConnectionID = Context.ConnectionId,
                            DeviceID = device.ID
                        });
                        await Clients.Client(Context.ConnectionId).SendAsync("Response", $"You are listening to device with id: {id}");

                    }
                    else
                    {
                        await Clients.Client(Context.ConnectionId).SendAsync("Response", $"You are fuck up");
                    }
                }
            }

        }
    }
}