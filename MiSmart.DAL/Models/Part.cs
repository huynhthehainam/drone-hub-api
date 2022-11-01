using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MiSmart.Infrastructure.Data;

namespace MiSmart.DAL.Models
{
    public class Part : EntityBase<Int32>
    {
        public Part() : base() { }
        public Part(ILazyLoader lazyLoader) : base(lazyLoader) { }
        public String? Group { get; set; }
        public String? Name { get; set; }
        private ICollection<LogResultDetail>? logResultDetails;
        public ICollection<LogResultDetail>? LogResultDetails
        {
            get => lazyLoader.Load(this, ref logResultDetails);
            set => logResultDetails = value;
        }
    }
}