
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Text.Json;
using MiSmart.Infrastructure.Data;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;

namespace MiSmart.DAL.Models
{
    public class Plan : EntityBase<Int64>
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
        public String Prefix { get; set; }
        public Point Location { get; set; }
        public Byte[] FileBytes { get; set; }
        public String FileName { get; set; }
        private Device device;
        [JsonIgnore]
        public Device Device
        {
            get => lazyLoader.Load(this, ref device);
            set => device = value;
        }
        public Int32 DeviceID { get; set; }
    }
}