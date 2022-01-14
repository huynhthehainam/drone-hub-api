

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using MiSmart.Infrastructure.Constants;
using MiSmart.Infrastructure.Data;
using MiSmart.Infrastructure.Helpers;
using NetTopologySuite.Geometries;

namespace MiSmart.DAL.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum DeviceStatus
    {
        Active,
        Inactive,

    }
    public class Device : EntityBase<Int32>
    {
        public Device() : base()
        {
        }

        public Device(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }

        public String Name { get; set; }
        public Guid UUID { get; set; } = Guid.NewGuid();
        public String Token { get; set; } = TokenHelper.GenerateToken();
        private ICollection<TelemetryRecord> records;
        [JsonIgnore]
        public ICollection<TelemetryRecord> Records
        {
            get => lazyLoader.Load(this, ref records);
            set => records = value;
        }
        public DeviceStatus Status { get; set; } = DeviceStatus.Active;
        private Team team;
        [JsonIgnore]
        public Team Team
        {
            get => lazyLoader.Load(this, ref team);
            set => team = value;
        }
        public Int64? TeamID { get; set; }

        private Customer customer;
        [JsonIgnore]
        public Customer Customer
        {
            get => lazyLoader.Load(this, ref customer);
            set => customer = value;
        }
        public Int32 CustomerID { get; set; }
        private ICollection<FlightStat> flightStats;
        [JsonIgnore]
        public ICollection<FlightStat> FlightStats
        {
            get => lazyLoader.Load(this, ref flightStats);
            set => flightStats = value;
        }
        private DeviceModel deviceModel;
        [JsonIgnore]
        public DeviceModel DeviceModel
        {
            get => lazyLoader.Load(this, ref deviceModel);
            set => deviceModel = value;
        }

        private ICollection<Plan> plans;
        [JsonIgnore]
        public ICollection<Plan> Plans
        {
            get => lazyLoader.Load(this, ref plans);
            set => plans = value;
        }
        public String AccessToken { get; set; }
        public DateTime? NextGeneratingAccessTokenTime { get; set; }
        public Int32 DeviceModelID { get; set; }
        public Point LastPoint { get; set; }
        public Double LastDirection { get; set; }
        public String LastAdditionalInformationString { get; set; }
        [NotMapped]
        public Object LastAdditionalInformation
        {
            get => LastAdditionalInformationString != null ? JsonSerializer.Deserialize<Object>(LastAdditionalInformationString, JsonOptions.CamelOptions) : null;
            set => LastAdditionalInformationString = JsonSerializer.Serialize(value, JsonOptions.CamelOptions);
        }
        public String GenerateDeviceAccessToken(String secretKey)
        {
            var claims = new[] { new Claim(Keys.JWTAuthKey, ID.ToString()) };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(claims: claims, signingCredentials: creds, expires: DateTime.Now.AddMonths(2));
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            return tokenString;
        }
    }

}