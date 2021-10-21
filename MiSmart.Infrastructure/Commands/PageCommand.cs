using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System;

namespace MiSmart.Infrastructure.Commands
{
    public class PageCommand
    {
        [Range(0, Int32.MaxValue)]
        public Int32? PageIndex { get; set; }
        [Range(0, Int32.MaxValue)]
        public Int32? PageSize { get; set; }
    }
}