
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Text.Json;
using MiSmart.Infrastructure.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiSmart.DAL.Models
{
    public class Plan : EntityBase<long>
    {
        public Plan() : base()
        {
        }

        public Plan(ILazyLoader lazyLoader) : base(lazyLoader)
        {
        }
        public String Name { get; set; }
        public Double Longitude { get; set; }
        public Double Latitude { get; set; }
        public Byte[] FileBytes { get; set; }
        public String FileName { get; set; }
    }
}