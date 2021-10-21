
using Microsoft.EntityFrameworkCore.Infrastructure;
using MiSmart.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MiSmart.DAL.Models
{
    public class Team : EntityBase<Int64>
    {
        public Team() : base()
        {
        }

        public Team(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }
        public String Name { get; set; }


        private Customer customer;
        [JsonIgnore]
        public Customer Customer
        {
            get => lazyLoader.Load(this, ref customer);
            set => customer = value;
        }
        public Int32 CustomerID { get; set; }



        private ICollection<TeamUser> teamUsers;
        [JsonIgnore]
        public ICollection<TeamUser> TeamUsers
        {
            get => lazyLoader.Load(this, ref teamUsers);
            set => teamUsers = value;
        }

        private ICollection<Device> devices;
        [JsonIgnore]
        public ICollection<Device> Devices
        {
            get => lazyLoader.Load(this, ref devices);
            set => devices = value;
        }
    }
}