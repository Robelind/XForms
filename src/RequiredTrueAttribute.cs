using System;
using System.ComponentModel.DataAnnotations;

namespace XForms
{
    public class RequiredTrueAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if(validationContext == null)
            {
                throw new ArgumentNullException(nameof(validationContext));
            }

            if(bool.TryParse(value.ToString(), out bool boolValue))
            {
                return(boolValue
                    ? ValidationResult.Success
                    : new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName), new[] { validationContext.DisplayName }));
            }
            else
            {
                throw new ArgumentException("Property type must be bool");
            }
        }
    }
}