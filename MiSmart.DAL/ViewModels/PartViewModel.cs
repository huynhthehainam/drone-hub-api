
using System;
using MiSmart.DAL.Models;
using MiSmart.Infrastructure.ViewModels;

namespace MiSmart.DAL.ViewModels
{
    public class PartViewModel : IViewModel<Part>
    {
        public Int32 ID {get; set; }
        public String? Group {get; set; }
        public String? Name {get; set; }
        public void LoadFrom(Part entity)
        {
            Group = entity.Group;
            Name = entity.Name;
            ID = entity.ID;
        }
    }
}