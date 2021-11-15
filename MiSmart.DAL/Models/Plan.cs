
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
        public String Name
        {
            get
            {
                var words = this.FileName.Split(".", StringSplitOptions.RemoveEmptyEntries);
                if (words.Length > 0)
                {
                    return words[0];
                }
                return null;
            }
        }
        public Double Longitude { get; set; }
        public Double Latitude { get; set; }
        public Byte[] FileBytes { get; set; }
        public String FileName { get; set; }
    }
}