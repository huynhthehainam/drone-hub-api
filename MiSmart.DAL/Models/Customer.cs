

using System;
using System.Collections.Generic;
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

        public String? Name { get; set; }
        public String? Address { get; set; }





        private ICollection<Field>? fields;
        public ICollection<Field>? Fields
        {
            get => lazyLoader.Load(this, ref fields);
            set => fields = value;
        }


        private ICollection<Device>? devices;
        public ICollection<Device>? Devices
        {
            get => lazyLoader.Load(this, ref devices);
            set => devices = value;
        }

        private ICollection<FlightStat>? flightStats;
        public ICollection<FlightStat>? FlightStats
        {
            get => lazyLoader.Load(this, ref flightStats);
            set => flightStats = value;
        }

        private ICollection<CustomerUser>? customerUsers;
        public ICollection<CustomerUser>? CustomerUsers
        {
            get => lazyLoader.Load(this, ref customerUsers);
            set => customerUsers = value;
        }



    }



}