using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;
namespace MiSmart.Infrastructure.Validations
{
    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly String[] extensions;
        public AllowedExtensionsAttribute(String[] extensions)
        {
            this.extensions = extensions;
        }
        protected override ValidationResult IsValid(Object value, ValidationContext validationContext)
        {
            var file = value as IFormFile;
            if (file != null)
            {
                var extension = Path.GetExtension(file.FileName);
                if (!extensions.Contains(extension.ToLower()))
                {
                    return new ValidationResult(GetErrorMessage());
                }
            }
            return ValidationResult.Success;
        }
        public String GetErrorMessage()
        {
            return $"This file extension is not allowed!";
        }
    }
}