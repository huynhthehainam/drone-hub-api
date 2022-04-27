


using System;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.ViewModels;

namespace MiSmart.DAL.ViewModels
{
    public class StreamingLinkViewMode : IViewModel<StreamingLink>
    {
        public Int32 ID { get; set; }
        public String DeviceName { get; set; }
        public String Link { get; set; }
        public DateTime CreatedTime { get; set; }
        public void LoadFrom(StreamingLink entity)
        {
            ID = entity.ID;
            DeviceName = entity.Device.Name;
            Link = entity.Link;
            CreatedTime = entity.CreatedTime;
        }
    }
}