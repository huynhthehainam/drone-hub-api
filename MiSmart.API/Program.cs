using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MiSmart.DAL.DatabaseContexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet.Server;
using MQTTnet.AspNetCore.Extensions;

namespace MiSmart.API
{
    public class Program
    {
        public static void Main(String[] args)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            CreateHostBuilder(args).Build().Run();
        }
        public static IHostBuilder CreateHostBuilder(String[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel(kestrelServerOptions =>
                        {
                            kestrelServerOptions.ListenAnyIP(1883,
                                listenOptions => listenOptions.UseMqtt());
                            kestrelServerOptions.ListenAnyIP(5000);
                        }
                    );
                    webBuilder.UseStartup<Startup>();
                });
    }
}
