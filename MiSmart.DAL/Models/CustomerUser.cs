
using System;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MiSmart.Infrastructure.Data;
namespace MiSmart.DAL.Models
{

    public class CustomerUser : EntityBase<Int64>
    {
        public CustomerUser() : base()
        {
        }

        public CustomerUser(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }

        public Int64 UserID { get; set; }
        private Customer customer;
        [JsonIgnore]
        public Customer Customer
        {
            get => lazyLoader.Load(this, ref customer);
            set => customer = value;
        }
        public Int32 CustomerID { get; set; }
    }
}