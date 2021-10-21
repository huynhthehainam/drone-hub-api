using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
namespace MiSmart.Infrastructure.Validations
{
    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly Int32 maxFileSize;
        public MaxFileSizeAttribute(Int32 maxFileSize)
        {
            this.maxFileSize = maxFileSize;
        }
        protected override ValidationResult IsValid(Object value, ValidationContext validationContext)
        {
            var file = value as IFormFile;
            if (file != null)
            {
                if (file.Length > maxFileSize)
                {
                    return new ValidationResult(GetErrorMessage());
                }
            }
            return ValidationResult.Success;
        }
        public String GetErrorMessage()
        {
            return $"Maximum allowed file size is { maxFileSize} bytes.";
        }
    }
}