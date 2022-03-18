

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MiSmart.Infrastructure.Data;
namespace MiSmart.DAL.Models
{
    public class Customer : EntityBase<Int32>
    {
        public Customer() : base()
        {

        }
        public Customer(ILazyLoader lazyLoader) : base(lazyLoader)
        {

        }

        public String Name { get; set; }
        public String Address { get; set; }





        private ICollection<Field> fields;
        [JsonIgnore]
        public ICollection<Field> Fields
        {
            get => lazyLoader.Load(this, ref fields);
            set => fields = value;
        }


        private ICollection<Device> devices;
        [JsonIgnore]
        public ICollection<Device> Devices
        {
            get => lazyLoader.Load(this, ref devices);
            set => devices = value;
        }

        private ICollection<FlightStat> flightStats;
        [JsonIgnore]
        public ICollection<FlightStat> FlightStats
        {
            get => lazyLoader.Load(this, ref flightStats);
            set => flightStats = value;
        }

        private ICollection<CustomerUser> customerUsers;
        [JsonIgnore]
        public ICollection<CustomerUser> CustomerUsers
        {
            get => lazyLoader.Load(this, ref customerUsers);
            set => customerUsers = value;
        }

       

    }



}