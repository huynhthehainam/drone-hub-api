


using System;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.ViewModels;

namespace MiSmart.DAL.ViewModels
{
    public class SmallCustomerViewModel : IViewModel<Customer>
    {
        public Int32 ID { get; set; }
        public String Name { get; set; }
        public String Address { get; set; }

        public void LoadFrom(Customer entity)
        {
            ID = entity.ID;
            Name = entity.Name;
            Address = entity.Address;
        }
    }
}