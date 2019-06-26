using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace XForms.Attributes
{
    public class RequiredIfTrueAttribute : ValidationAttribute
    {
        private readonly string _otherProperty;

        public RequiredIfTrueAttribute(string otherProperty)
        {
            if(string.IsNullOrWhiteSpace(otherProperty))
            {
                throw new ArgumentException($"{nameof(otherProperty)} cannot be null or empty");
            }
            _otherProperty = otherProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if(validationContext == null)
            {
                throw new ArgumentNullException(nameof(validationContext));
            }

            PropertyInfo otherProperty = validationContext.ObjectType.GetProperty(_otherProperty);

            if(otherProperty == null)
            {
                throw new ArgumentException($"Could not find a property named '{_otherProperty}'.");
            }

            if(!bool.TryParse(otherProperty.GetValue(validationContext.ObjectInstance).ToString(), out bool otherValue))
            {
                throw new ArgumentException("Dependant property type must be bool");
            }

            return(!otherValue || !string.IsNullOrWhiteSpace(value?.ToString())
                ? ValidationResult.Success
                : new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName), new[] { validationContext.DisplayName }));
        }
    }
}
