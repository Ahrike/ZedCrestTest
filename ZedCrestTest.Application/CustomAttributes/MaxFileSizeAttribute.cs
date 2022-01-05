using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Api.CustomAttributes
{
    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly int _maxFileSize;
        public MaxFileSizeAttribute(int maxFileSize)
        {
            _maxFileSize = maxFileSize;
        }

        protected override ValidationResult IsValid(
        object value, ValidationContext validationContext)
        {
            var files = value as IFormFileCollection;
            if (files != null)
            {
                foreach (var file in files)
                {
                    if (file.Length > _maxFileSize)
                    {
                        return new ValidationResult(GetErrorMessage());
                    }
                }
                
            }

            return ValidationResult.Success;
        }

        public string GetErrorMessage()
        {
            return $"An error occured. The Maximum file size allowed is { _maxFileSize} bytes.";
        }
    }
}
