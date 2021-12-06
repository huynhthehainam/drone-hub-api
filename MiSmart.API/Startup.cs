using MiSmart.DAL.DatabaseContexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Http;
using System.IO;
using MiSmart.Infrastructure.Constants;
using MiSmart.Infrastructure.Services;
using MiSmart.Infrastructure.Settings;
using Microsoft.OpenApi.Models;
using MiSmart.Infrastructure.Extensions;
using System.Collections.Generic;
using MiSmart.Infrastructure.Swagger;
using MiSmart.Infrastructure.Responses;
using System;
using System.Linq;
using MiSmart.Infrastructure.Middlewares;
using MiSmart.API.Protos;
using System.Net.Http;

using MQTTnet.AspNetCore.Extensions;
using MQTTnet.AspNetCore;
using MQTTnet.AspNetCore.AttributeRouting;
using MiSmart.API.Services;
using MiSmart.API.MqttControllers;
using MiSmart.Infrastructure.Mqtt;
using MiSmart.DAL.Repositories;

using Microsoft.AspNetCore.SignalR;
using System.Reflection;
using MiSmart.API.Settings;
using Npgsql.EntityFrameworkCore.PostgreSQL.Storage.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using MiSmart.DAL.Models;
using NetTopologySuite;
using NetTopologySuite.Geometries;
namespace MiSmart.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            // services.AddSingleton<IRelationalTypeMappingSource,NpgsqlTypeMappingSource>();
            services.AddEntityFrameworkNpgsqlNetTopologySuite().AddEntityFrameworkNpgsql().AddDbContext<DatabaseContext>((sp, opt) => opt.UseNpgsql(Configuration.GetConnectionString(DbConnection.DatabaseKey), b =>
            {
                b.UseNetTopologySuite();
                b.MigrationsAssembly("MiSmart.API");

            }).UseInternalServiceProvider(sp));
            services.AddDistributedRedisCache(option =>
            {
                option.Configuration = Configuration["RedisSettings:Host"] + ":" + Configuration["RedisSettings:Port"] + ",connectTimeout=10000,syncTimeout=10000";
            });
            services.AddMvc().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.IgnoreNullValues = true;
            });
            services.AddSwagger(Configuration, new OpenApiInfo
            {
                Title = "Drone hub management",
                Version = "v1",
                Description = "Drone hub management"
            });
            services.AddHttpClient();



            #region  ConfiguringMqtt
            services.AddSingleton<IMqttService, MqttService>();
            services
               .AddHostedMqttServerWithServices(aspNetMqttServerOptionsBuilder =>
               {
                   var mqttService = aspNetMqttServerOptionsBuilder.ServiceProvider.GetRequiredService<IMqttService>();
                   mqttService.ConfigureMqttServerOptions(aspNetMqttServerOptionsBuilder);
                   aspNetMqttServerOptionsBuilder.Build();
               })
               .AddMqttConnectionHandler()
               .AddConnections()
               .AddMqttWebSocketServerAdapter();
            services.AddMqttControllers();
            // services.AddScoped<MqttBaseController, MqttController>();
            // services.AddScoped<MqttBaseController, DevicesController>();

            #endregion




            #region ConfiguringGrpc
            services.AddGrpc();
            services.AddGrpcClient<AuthProtoService.AuthProtoServiceClient>(o => o.Address = new Uri(Configuration["GrpcConfigs:AuthUrl"])).ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new HttpClientHandler { ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true };
            });


            #endregion

            #region ConfiguringSettings
            services.Configure<ActionResponseSettings>(Configuration.GetSection("ActionResponseSettings"));
            services.Configure<AuthSettings>(Configuration.GetSection("AuthSettings"));
            services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));
            services.Configure<SmtpSettings>(Configuration.GetSection("SmtpSettings"));
            services.Configure<HashSettings>(Configuration.GetSection("HashSettings"));
            services.Configure<KeySettings>(Configuration.GetSection("KeySettings"));
            services.Configure<ExpiredTimeSettings>(Configuration.GetSection("ExpiredTimeSettings"));
            services.Configure<ConversionSettings>(Configuration.GetSection("ConversionSettings"));

            #endregion
            #region ConfiguringJWT
            services.ConfigureJWTAuthentication(Configuration);
            #endregion
            #region AddingServices
            services.AddSingleton<HashService, HashService>();
            services.AddScoped<JWTService, JWTService>();
            services.AddScoped<CacheService, CacheService>();
            services.AddScoped<EmailService, EmailService>();

            #endregion
            #region AddingFactories

            services.AddSingleton<IActionResponseFactory, ActionResponseFactory>();
            #endregion


            #region AddingRepositories

            services.AddScoped<CustomerRepository, CustomerRepository>();
            services.AddScoped<TeamRepository, TeamRepository>();
            services.AddScoped<DeviceRepository, DeviceRepository>();
            services.AddScoped<TeamUserRepository, TeamUserRepository>();
            services.AddScoped<PlanRepository, PlanRepository>();

            services.AddScoped<CustomerUserRepository, CustomerUserRepository>();
            services.AddScoped<TelemetryRecordRepository, TelemetryRecordRepository>();
            services.AddScoped<DeviceModelRepository, DeviceModelRepository>();
            services.AddScoped<FlightStatRepository, FlightStatRepository>();
            services.AddScoped<FieldRepository, FieldRepository>();


            #endregion


            #region AddingCors

            var customCorsUrls = new List<String>() { "http://localhost:4200", "http://localhost:3000" };
            services.AddCors(options =>
            {
                options.AddPolicy(name: Keys.AllowedOrigin, builder =>
                {
                    builder.WithOrigins(
                    customCorsUrls.ToArray()
                    ).AllowAnyHeader().AllowAnyMethod().AllowCredentials();
                });
            });
            #endregion

            #region ConfiguringWebSocket


            // services.AddSignalR();

            #endregion



            #region AddingCronJobs


            #endregion
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, DatabaseContext databaseContext, HashService hashService)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.CustomizeErrorHandler("App flighthub");
            }


            if (!Directory.Exists(FolderPaths.StaticFilePath))
            {
                Directory.CreateDirectory(FolderPaths.StaticFilePath);
            }
            if (!Directory.Exists(Path.Combine(FolderPaths.StaticFilePath, "images")))
            {
                Directory.CreateDirectory(Path.Combine(FolderPaths.StaticFilePath, "images"));
            }
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), FolderPaths.StaticFilePath)),
                RequestPath = new PathString("/staticfiles"),
            });
            app.UseSwagger().UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/swagger/v1/swagger.json", "Flighthub.API V1");
            });
            SeedData(databaseContext, hashService);

            app.UseRouting();
            app.UseCors("AllowedOrigin");
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<RemoteAuthMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
            app.UseMqttServer(server =>
               app.ApplicationServices.GetRequiredService<IMqttService>().ConfigureMqttServer(server));
        }
        public void SeedData(DatabaseContext context, HashService hashService)
        {
            context.Database.Migrate();
            if (!context.DeviceModels.Any())
            {
                var deviceModel1 = new DeviceModel { Name = "VS20" };
                context.DeviceModels.AddRange(new DeviceModel[] { deviceModel1 });
            }
            if (!context.Customers.Any())
            {
                var customer1 = new Customer
                {
                    Name = "MiSmart",
                    Address = "Quận 9, TP Thủ Đức",
                };

                context.Customers.AddRange(new Customer[] { customer1 });
                context.SaveChanges();

                var customerUser1 = new CustomerUser { UserID = 1, Customer = customer1, Type = CustomerMemberType.Owner };
                context.CustomerUsers.AddRange(new CustomerUser[] { customerUser1 });
                context.SaveChanges();

                var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

                var deviceModel = context.DeviceModels.FirstOrDefault(ww => ww.ID == 1);
                if (deviceModel is not null)
                {
                    var device1 = new Device { Customer = customer1, DeviceModel = deviceModel, Name = "Test drone 1",  };
                    context.Devices.AddRange(new Device[] { device1 });
                    context.SaveChanges();

                    var random = new Random();
                    var flightStats = new List<FlightStat>();
                    for (var i = 0; i < 50; i++)
                    {
                        FlightStat flightStat = new FlightStat
                        {
                            CreatedTime = DateTime.Now,
                            Customer = customer1,
                            Device = device1,
                            DeviceName = device1.Name,
                            FieldName = "Long An",
                            Flights = random.Next(0, 20),
                            TaskArea = random.NextDouble() * 1000,
                            PilotName = "",
                            TaskAreaUnit = AreaUnit.Hectare,
                            TaskLocation = "Bến Lức",
                            FlightDuration = random.NextDouble() * 100,
                            FlywayPoints = geometryFactory.CreateLineString(new Coordinate[]{new Coordinate(106.090684,10.711697),
                            new Coordinate(106.099201 +( random.NextDouble()/100),10.712229 + ( random.NextDouble()/100)),
                            new Coordinate( 106.099237+( random.NextDouble()/100),10.711200+( random.NextDouble()/100)),
                            new Coordinate( 106.095195+( random.NextDouble()/100),10.710917+( random.NextDouble()/100)),
    new Coordinate( 106.093679,10.709746)
                            }),
                        };
                        flightStats.Add(flightStat);
                    }

                    context.FlightStats.AddRange(flightStats);
                    context.SaveChanges();

                }

            }

        }
    }
}
