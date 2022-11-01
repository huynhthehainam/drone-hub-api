

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using MiSmart.Infrastructure.Constants;
using MiSmart.Infrastructure.Data;
using MiSmart.Infrastructure.Helpers;

namespace MiSmart.DAL.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum DeviceStatus
    {
        Active,
        Inactive,
        Offline
    }
    public class Device : EntityBase<Int32>
    {
        public Device() : base()
        {
        }

        public Device(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }

        public String? Name { get; set; }
        public Guid UUID { get; set; } = Guid.NewGuid();
        public String? Token { get; set; } = TokenHelper.GenerateToken();

        public DeviceStatus Status { get; set; } = DeviceStatus.Offline;
        private Team? team;
        public Team? Team
        {
            get => lazyLoader.Load(this, ref team);
            set => team = value;
        }
        public Int64? TeamID { get; set; }

        private Customer? customer;
        public Customer? Customer
        {
            get => lazyLoader.Load(this, ref customer);
            set => customer = value;
        }
        public Int32 CustomerID { get; set; }

        private ExecutionCompany? executionCompany;
        public ExecutionCompany? ExecutionCompany
        {
            get => lazyLoader.Load(this, ref executionCompany);
            set => executionCompany = value;
        }
        public Int32? ExecutionCompanyID { get; set; }
        private ICollection<FlightStat>? flightStats;
        public ICollection<FlightStat>? FlightStats
        {
            get => lazyLoader.Load(this, ref flightStats);
            set => flightStats = value;
        }
        private DeviceModel? deviceModel;
        public DeviceModel? DeviceModel
        {
            get => lazyLoader.Load(this, ref deviceModel);
            set => deviceModel = value;
        }

        private ICollection<Plan>? plans;
        public ICollection<Plan>? Plans
        {
            get => lazyLoader.Load(this, ref plans);
            set => plans = value;
        }
        public String? AccessToken { get; set; }
        public DateTime? NextGeneratingAccessTokenTime { get; set; }
        public Int32 DeviceModelID { get; set; }

        private ICollection<TelemetryGroup>? telemetryGroups;
        public ICollection<TelemetryGroup>? TelemetryGroups
        {
            get => lazyLoader.Load(this, ref telemetryGroups);
            set => telemetryGroups = value;
        }
        public DateTime? LastOnline { get; set; }


        private TelemetryGroup? lastGroup;
        public TelemetryGroup? LastGroup
        {
            get => lazyLoader.Load(this, ref lastGroup);
            set => lastGroup = value;
        }
        public Guid? LastGroupID { get; set; }

        private ICollection<LogFile>? logFiles;
        public ICollection<LogFile>? LogFiles
        {
            get => lazyLoader.Load(this, ref logFiles);
            set => logFiles = value;
        }
        public List<Guid>? LastBatterGroupLogs
        {
            get; set;
        }

        public String GenerateDeviceAccessToken(String secretKey)
        {
            var claims = new[] { new Claim(Keys.JWTAuthKey, ID.ToString()), new Claim(Keys.JWTUserTypeKey, "Device") };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(claims: claims, signingCredentials: creds, expires: DateTime.UtcNow.AddMonths(2));
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return tokenString;
        }


        private ICollection<StreamingLink>? streamingLinks;
        public ICollection<StreamingLink>? StreamingLinks
        {
            get => lazyLoader.Load(this, ref streamingLinks);
            set => streamingLinks = value;
        }

        private ICollection<MaintenanceReport>? maintenanceReports;
        public ICollection<MaintenanceReport>? MaintenanceReports
        {
            get => lazyLoader.Load(this, ref maintenanceReports);
            set => maintenanceReports = value;
        }
    }

}