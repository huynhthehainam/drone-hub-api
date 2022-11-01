using System;
using System.ComponentModel.DataAnnotations;

namespace MiSmart.API.Commands
{
    public class AddingStreamingLinkCommand
    {
        [Required]
        public String? Link { get; set; }
    }
}