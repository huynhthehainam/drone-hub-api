using MiSmart.DAL.DatabaseContexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
using MQTTnet.AspNetCore.AttributeRouting;
using MiSmart.API.Services;
using MiSmart.Infrastructure.Mqtt;
using MiSmart.DAL.Repositories;
using MiSmart.API.Settings;
using MiSmart.DAL.Models;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using MiSmart.API.Models;
using Microsoft.Extensions.Options;
using MiSmart.Infrastructure.RabbitMQ;
using MiSmart.Microservices.OrderService.RabbitMQ;
using MiSmart.Infrastructure.QueuedBackgroundTasks;
using MiSmart.API.QueuedServices;
using Microsoft.Extensions.Hosting;
using MiSmart.API.GrpcServices;

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


            #endregion

            #region QueuedTasks

            // services.AddHostedService<QueuedHostedService1>();
            // services.AddHostedService<QueuedHostedService2>();


            // services.AddSingleton<IBackgroundTaskQueue>(ctx =>
            // {
            //     if (!int.TryParse(Configuration["QueueCapacity"], out var queueCapacity))
            //         queueCapacity = 1000;
            //     return new BackgroundTaskQueue(queueCapacity);
            // });
            #endregion




            #region ConfiguringGrpc
            services.AddGrpc();
            services.AddGrpcClient<AuthProtoService.AuthProtoServiceClient>(o => o.Address = new Uri(Configuration["GrpcConfigs:AuthUrl"])).ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new HttpClientHandler { ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true, };
            });

            services.AddSingleton<AuthGrpcClientService, AuthGrpcClientService>();


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

            #region RabbitMQ
            services.AddRabbit(Configuration);
            services.AddHostedService<ConsumeAuthRabbitMQHostedService>();


            #endregion


            #region AddingCors

            var customCorsUrls = new List<String>() { "http://localhost:4200", "http://localhost:3000", "https://dronehub.mismart.ai" };
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
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.CustomizeErrorHandler("App dronehub");
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
            SeedData(app);

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
        public void SeedData(IApplicationBuilder app)
        {
            var scope = app.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
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
            }

            if (!context.CustomerUsers.Any())
            {
                var customer = context.Customers.FirstOrDefault(ww => ww.ID == 1);
                if (customer is not null)
                {
                    var customerUser1 = new CustomerUser { UserID = 1, Customer = customer, Type = CustomerMemberType.Owner };
                    var customerUser2 = new CustomerUser { UserID = 2, Customer = customer, Type = CustomerMemberType.Member };
                    var customerUser3 = new CustomerUser { UserID = 3, Customer = customer, Type = CustomerMemberType.Member };
                    var customerUser4 = new CustomerUser { UserID = 4, Customer = customer, Type = CustomerMemberType.Member };

                    context.CustomerUsers.AddRange(new CustomerUser[] { customerUser1, customerUser2, customerUser3, customerUser4 });
                    context.SaveChanges();


                }
            }
            if (!context.Teams.Any())
            {
                var customer = context.Customers.FirstOrDefault(ww => ww.ID == 1);
                if (customer is not null)
                {
                    var team1 = new Team { Customer = customer, Name = "Team 1", };
                    var team2 = new Team { Customer = customer, Name = "Team 2", };
                    var team3 = new Team { Customer = customer, Name = "Team 3", };

                    context.Teams.AddRange(new Team[] { team1, team2, team3 });
                    context.SaveChanges();
                }
            }

            if (!context.TeamUsers.Any())
            {
                var team1 = context.Teams.FirstOrDefault(ww => ww.ID == 1);
                var team2 = context.Teams.FirstOrDefault(ww => ww.ID == 2);
                var team3 = context.Teams.FirstOrDefault(ww => ww.ID == 3);

                var customerUser1 = context.CustomerUsers.FirstOrDefault(ww => ww.ID == 1);
                var customerUser2 = context.CustomerUsers.FirstOrDefault(ww => ww.ID == 2);
                var customerUser3 = context.CustomerUsers.FirstOrDefault(ww => ww.ID == 3);
                var customerUser4 = context.CustomerUsers.FirstOrDefault(ww => ww.ID == 4);
                if (team1 is not null && team2 is not null && team3 is not null && customerUser1 is not null
                && customerUser2 is not null && customerUser3 is not null && customerUser4 is not null)
                {
                    TeamUser teamUser1 = new TeamUser { CustomerUser = customerUser1, Team = team1 };
                    TeamUser teamUser2 = new TeamUser { CustomerUser = customerUser2, Team = team2 };
                    TeamUser teamUser3 = new TeamUser { CustomerUser = customerUser3, Team = team3 };
                    TeamUser teamUser4 = new TeamUser { CustomerUser = customerUser4, Team = team2 };
                    context.TeamUsers.AddRange(new TeamUser[] { teamUser1, teamUser2, teamUser3, teamUser4 });
                    context.SaveChanges();
                }
            }



            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);


            if (!context.Devices.Any())
            {
                AuthSettings authSettings = scope.ServiceProvider.GetRequiredService<IOptions<AuthSettings>>().Value;
                var customer = context.Customers.FirstOrDefault(ww => ww.ID == 1);
                if (customer is not null)
                {
                    var deviceModel = context.DeviceModels.FirstOrDefault(ww => ww.ID == 1);
                    if (deviceModel is not null)
                    {
                        var team1 = context.Teams.FirstOrDefault(ww => ww.ID == 1);
                        var team2 = context.Teams.FirstOrDefault(ww => ww.ID == 2);
                        var team3 = context.Teams.FirstOrDefault(ww => ww.ID == 3);
                        if (team1 is not null && team2 is not null && team3 is not null)
                        {
                            var device1 = new Device { Customer = customer, DeviceModel = deviceModel, Name = "Test drone 1", Team = team1 };
                            var device2 = new Device { Customer = customer, DeviceModel = deviceModel, Name = "Test drone 2", Team = team2 };
                            var device3 = new Device { Customer = customer, DeviceModel = deviceModel, Name = "Test drone 3", Team = team3 };

                            device1.AccessToken = device1.GenerateDeviceAccessToken(authSettings.AuthSecret);
                            device2.AccessToken = device2.GenerateDeviceAccessToken(authSettings.AuthSecret);
                            device3.AccessToken = device3.GenerateDeviceAccessToken(authSettings.AuthSecret);

                            context.Devices.AddRange(new Device[] { device1, device2, device3 });
                            context.SaveChanges();
                        }
                    }



                }
            }
            var random = new Random();
            if (!context.FlightStats.Any())
            {
                var customer = context.Customers.FirstOrDefault(ww => ww.ID == 1);
                if (customer is not null)
                {
                    var device = context.Devices.FirstOrDefault(ww => ww.ID == 1);
                    if (device is not null)
                    {
                        var flightStats = new List<FlightStat>();
                        for (var i = 0; i < 50; i++)
                        {
                            FlightStat flightStat = new FlightStat
                            {
                                CreatedTime = DateTime.Now,
                                Customer = customer,
                                Device = device,
                                DeviceName = device.Name,
                                FieldName = "Long An",
                                Flights = random.Next(0, 20),
                                TaskArea = random.NextDouble() * 1000,
                                PilotName = "",
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
            if (!context.TelemetryRecords.Any())
            {
                var customer = context.Customers.FirstOrDefault(ww => ww.ID == 1);
                if (customer is not null)
                {
                    var device = context.Devices.FirstOrDefault(ww => ww.ID == 1);
                    if (device is not null)
                    {
                        var samples = new List<LocationSample>(){
                           new LocationSample {Latitude = 20.991331, Longitude= 105.803306},
                            new LocationSample {Latitude = 20.990926, Longitude= 105.803625},
                            new LocationSample {Latitude = 20.990418, Longitude= 105.803871},
                            new LocationSample {Latitude = 20.989956, Longitude= 105.804079},
                            new LocationSample {Latitude = 20.989868, Longitude= 105.80393},
                            new LocationSample {Latitude = 20.989829, Longitude= 105.803878},
                            new LocationSample {Latitude = 20.989617, Longitude= 105.803545},
                            new LocationSample {Latitude = 20.989517, Longitude= 105.803379},
                            new LocationSample {Latitude = 20.989414, Longitude= 105.803188},
                            new LocationSample {Latitude = 20.989293, Longitude= 105.803},
                            new LocationSample {Latitude = 20.989195, Longitude= 105.802838},
                            new LocationSample {Latitude = 20.989143, Longitude= 105.802641},
                            new LocationSample {Latitude = 20.989189, Longitude= 105.802412},
                            new LocationSample {Latitude = 20.989215, Longitude= 105.802256},
                            new LocationSample {Latitude = 20.989246, Longitude= 105.802038},
                            new LocationSample {Latitude = 20.989281, Longitude= 105.801827},
                            new LocationSample {Latitude = 20.989337, Longitude= 105.801753},
                            new LocationSample {Latitude = 20.989495, Longitude= 105.801948},
                            new LocationSample {Latitude = 20.98963, Longitude= 105.802107},
                            new LocationSample {Latitude = 20.98978, Longitude= 105.802249},
                            new LocationSample {Latitude = 20.989985, Longitude= 105.802109},
                            new LocationSample {Latitude = 20.990153, Longitude= 105.802023},
                            new LocationSample {Latitude = 20.990339, Longitude= 105.801903},
                            new LocationSample {Latitude = 20.990479, Longitude= 105.802099},
                            new LocationSample {Latitude = 20.990697, Longitude= 105.802412},
                            new LocationSample {Latitude = 20.990895, Longitude= 105.802726},
                            new LocationSample {Latitude = 20.991, Longitude= 105.802887},
                            new LocationSample {Latitude = 20.991135, Longitude= 105.803118}
                        };

                        var telemetryRecords = new List<TelemetryRecord>();
                        foreach (var sample in samples)
                        {
                            var record = new TelemetryRecord
                            {
                                Device = device,
                                CreatedTime = DateTime.Now,
                                LocationPoint = geometryFactory.CreatePoint(new Coordinate(sample.Longitude, sample.Latitude)),
                                AdditionalInformation = new { Speed = 2.0 },
                            };
                            telemetryRecords.Add(record);
                        }
                        context.TelemetryRecords.AddRange(telemetryRecords);
                        context.SaveChanges();
                    }
                }
            }
            if (!context.Fields.Any())
            {
                var customer = context.Customers.FirstOrDefault(ww => ww.ID == 1);
                if (customer is not null)
                {
                    var samples = new List<LocationSample>(){
                        new LocationSample{
                            Latitude = 10.667356525074577,
                            Longitude = 105.53553301447937
                        },

                        new LocationSample{
                            Latitude = 10.667569635345581,
                            Longitude =  105.53340716277171,
                        },new LocationSample{
                            Latitude = 10.668470393848125,
                            Longitude =105.53359234574434,
                        },new LocationSample{
                            Latitude = 10.66834022041227,
                            Longitude = 105.53570373793264,
                        }, new LocationSample{
                            Latitude = 10.667356525074577,
                            Longitude = 105.53553301447937
                        },
                    };

                    var samples2 = new List<LocationSample>(){
                       new LocationSample {
                         Latitude= 10.655949865583352,
                        Longitude= 105.56631240606391,

                    },
                        new LocationSample {
                                Latitude= 10.655771875351734,
                                Longitude= 105.56736843238615,

                            },
                        new LocationSample {
                                Latitude= 10.654762013237915,
                                Longitude= 105.56733991624013,

                            },
                        new LocationSample {
                                Latitude= 10.654708835812956,
                                Longitude= 105.56669797113786,

                            },
                        new LocationSample {
                                Latitude= 10.654944207304224,
                                Longitude= 105.56668806266627,

                            },
                        new LocationSample {
                                Latitude= 10.655010570279309,
                                Longitude= 105.56661847908566,

                            },
                        new LocationSample {
                                Latitude= 10.655058048827712,
                                Longitude= 105.56625073949712,

                            },new LocationSample {
                         Latitude= 10.655949865583352,
                        Longitude= 105.56631240606391,

                    },
                                            };
                    var coordinates = samples.Select(sample => new Coordinate(sample.Longitude, sample.Latitude)).ToArray();
                    var field1 = new Field
                    {
                        FieldName = "Cô bảy",
                        FieldLocation = "Long An",
                        MappingArea = 1000,
                        MappingTime = 120,
                        IsLargeFarm = true,
                        WorkArea = 1200,
                        UpdatedTime = DateTime.Now,
                        Name = "Cô bảy long an",
                        WorkSpeed = 12,
                        InnerArea = 800,
                        PilotName = "Thanh Hà",
                        SprayWidth = 5,
                        SprayDir = 5,

                        Border = geometryFactory.CreatePolygon(coordinates),
                        Flyway = geometryFactory.CreateLineString(samples.Select(sample => new Coordinate(sample.Longitude, sample.Latitude)).ToArray()),
                        CreatedTime = DateTime.Now,
                        Customer = customer,
                        EdgeOffset = 0,
                        LocationPoint = geometryFactory.CreatePoint(new Coordinate(samples[0].Longitude, samples[0].Latitude)),
                    };
                    var field2 = new Field
                    {
                        FieldName = "Cô tám",
                        FieldLocation = "Long Điền",
                        MappingArea = 1100,
                        MappingTime = 130,
                        IsLargeFarm = true,
                        WorkArea = 1400,
                        UpdatedTime = DateTime.Now,
                        Name = "Cô bảy long điền",
                        WorkSpeed = 10,
                        InnerArea = 900,
                        PilotName = "Thanh Trúc",
                        SprayWidth = 5,
                        SprayDir = 5,

                        Border = geometryFactory.CreatePolygon(samples2.Select(sample => new Coordinate(sample.Longitude, sample.Latitude)).ToArray()),
                        Flyway = geometryFactory.CreateLineString(samples.Select(sample => new Coordinate(sample.Longitude, sample.Latitude)).ToArray()),
                        CreatedTime = DateTime.Now,
                        Customer = customer,
                        EdgeOffset = 0,
                        LocationPoint = geometryFactory.CreatePoint(new Coordinate(samples2[0].Longitude, samples2[0].Latitude)),
                    };

                    context.Fields.AddRange(new Field[] { field1, field2 });
                    context.SaveChanges();

                }
            }

        }

    }

}
