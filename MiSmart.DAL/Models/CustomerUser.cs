
using System;
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

        public Guid UserUUID { get; set; }
        private Customer? customer;
        public Customer? Customer
        {
            get => lazyLoader.Load(this, ref customer);
            set => customer = value;
        }
        public Int32 CustomerID { get; set; }
    }
}