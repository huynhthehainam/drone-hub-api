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
using MiSmart.Infrastructure.Middleware;

using MQTTnet.AspNetCore.Extensions;
using MQTTnet.AspNetCore.AttributeRouting;
using MiSmart.API.Services;
using MiSmart.Infrastructure.Mqtt;
using MiSmart.DAL.Repositories;
using MiSmart.API.Settings;
using MiSmart.Infrastructure.RabbitMQ;
using Microsoft.Extensions.Hosting;
using MiSmart.Infrastructure.Minio;
using MiSmart.Infrastructure.ScheduledTasks;
using MiSmart.API.ScheduledTasks;
using FirebaseAdmin;
using MiSmart.API.RabbitMQ;
using Microsoft.Extensions.Options;

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
                options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
            });
            services.AddSwagger(Configuration, new OpenApiInfo
            {
                Title = "Drone hub management",
                Version = "v1",
                Description = "Drone hub management"
            });
            services.AddHttpClient();

            FirebaseApp.Create();



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


            services.AddMinio(Configuration);
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
            // services.AddGrpc();
            // services.AddGrpcClient<AuthProtoService.AuthProtoServiceClient>(o => o.Address = new Uri(Configuration["GrpcConfigs:AuthUrl"])).ConfigurePrimaryHttpMessageHandler(() =>
            // {
            //     return new HttpClientHandler { ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true, };
            // });

            // services.AddSingleton<AuthGrpcClientService, AuthGrpcClientService>();


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
            services.Configure<AuthSystemSettings>(Configuration.GetSection("AuthSystemSettings"));
            services.Configure<FrontEndSettings>(Configuration.GetSection("FrontEndSettings"));
            services.Configure<FarmAppSettings>(Configuration.GetSection("FarmAppSettings"));
            services.Configure<RpanionSettings>(Configuration.GetSection("RpanionSettings"));
            services.Configure<TargetEmailSettings>(Configuration.GetSection("TargetEmailSettings"));

            #endregion
            #region ConfiguringJWT
            services.ConfigureJWTAuthentication(Configuration);
            #endregion
            #region AddingServices
            services.AddSingleton<HashService, HashService>();
            services.AddScoped<JWTService, JWTService>();
            services.AddScoped<MyEmailService, MyEmailService>();

            services.AddSingleton<AuthSystemService, AuthSystemService>();

            services.AddSingleton<CountingService, CountingService>();

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
            services.AddScoped<TelemetryGroupRepository, TelemetryGroupRepository>();
            services.AddScoped<ExecutionCompanyUserRepository, ExecutionCompanyUserRepository>();
            services.AddScoped<ExecutionCompanyRepository, ExecutionCompanyRepository>();
            services.AddScoped<BatteryGroupLogRepository, BatteryGroupLogRepository>();
            services.AddScoped<BatteryRepository, BatteryRepository>();
            services.AddScoped<BatteryModelRepository, BatteryModelRepository>();
            services.AddScoped<ExecutionCompanyUserFlightStatRepository, ExecutionCompanyUserFlightStatRepository>();
            services.AddScoped<ExecutionCompanySettingRepository, ExecutionCompanySettingRepository>();
            services.AddScoped<StreamingLinkRepository, StreamingLinkRepository>();
            services.AddScoped<MaintenanceReportRepository, MaintenanceReportRepository>();
            services.AddScoped<FlightStatReportRecordRepository, FlightStatReportRecordRepository>();
            services.AddScoped<LogFileRepository, LogFileRepository>();
            services.AddScoped<LogReportRepository, LogReportRepository>();
            services.AddScoped<LogDetailRepository, LogDetailRepository>();
            services.AddScoped<LogReportResultRepository, LogReportResultRepository>();
            services.AddScoped<LogTokenRepository, LogTokenRepository>();
            services.AddScoped<PartRepository, PartRepository>();
            services.AddScoped<LogResultDetailRepository, LogResultDetailRepository>();

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

            services.AddCronJob<RemovingOldRecordsTask>(o =>
            {
                o.CronExpression = "* */30 * * * *";
                o.TimeZoneInfo = TimeZoneInfo.Utc;
            });

            services.AddCronJob<UpdatingTaskBingLocation>(o =>
            {
                o.CronExpression = "*/10 * * * * *";
                o.TimeZoneInfo = TimeZoneInfo.Utc;
            });
            services.AddCronJob<UpdatingFlightStatBoundary>(o =>
            {
                o.CronExpression = "*/30 * * * * *";
                o.TimeZoneInfo = TimeZoneInfo.Utc;
            });

            services.AddCronJob<SendingDailyLowBatteryReport>(o =>
            {
                o.CronExpression = "0 0 15 * * *";
                o.TimeZoneInfo = TimeZoneInfo.Utc;
            });
            services.AddCronJob<SynchronizingLog>(o =>
            {
                o.CronExpression = "*/20 * * * * *";
                o.TimeZoneInfo = TimeZoneInfo.Utc;
            });
            services.AddCronJob<RemovingTimeOutToken>(o =>
            {
                o.CronExpression = "*/10 * * * * *";
                o.TimeZoneInfo = TimeZoneInfo.Utc;
            });
            services.AddCronJob<UpdatingLogDetail>(o =>
            {
                o.CronExpression = "*/20 * * * * *";
                o.TimeZoneInfo = TimeZoneInfo.Utc;
            });
            services.AddCronJob<UpdatingLogBingLocation>(o =>
            {
                o.CronExpression = "*/20 * * * * *";
                o.TimeZoneInfo = TimeZoneInfo.Utc;
            });



            //     services.AddCronJob<UpdatingCostFlightStatsTask>(o =>
            //    {
            //        o.CronExpression = "*/50 * * * * *";
            //        o.TimeZoneInfo = TimeZoneInfo.Utc;
            //    });
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
                app.CustomizeErrorHandler("App Drone Hub");
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
                c.SwaggerEndpoint($"/swagger/v1/swagger.json", "FlightHub.API V1");
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

            var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

            TargetEmailSettings targetEmailSettings = scope.ServiceProvider.GetRequiredService<IOptions<TargetEmailSettings>>().Value;
            foreach (var email in targetEmailSettings.LowBattery)
            {
                Console.WriteLine(email);
            }
            if (!env.IsDevelopment())
            {
                Console.WriteLine("Not dev env");
                return;
            }

        }

    }

}
