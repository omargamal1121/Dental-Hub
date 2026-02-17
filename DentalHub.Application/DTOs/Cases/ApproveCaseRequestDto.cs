using System.ComponentModel.DataAnnotations;

namespace DentalHub.Application.DTOs.Cases
{
    /// DTO for approving/rejecting a case request
    public class ApproveCaseRequestDto
    {
        [Required(ErrorMessage = "Request public ID is required")]
        public string RequestId { get; set; }

        [Required(ErrorMessage = "Doctor public ID is required")]
        public string DoctorId { get; set; }
    }

    /// Custom validation attribute for conditional required fields
    public class RequiredIfAttribute : ValidationAttribute
    {
        private readonly string _propertyName;
        private readonly object _value;

        public RequiredIfAttribute(string propertyName, object value)
        {
            _propertyName = propertyName;
            _value = value;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var property = validationContext.ObjectType.GetProperty(_propertyName);
            if (property == null)
                return new ValidationResult($"Property {_propertyName} not found");

            var propertyValue = property.GetValue(validationContext.ObjectInstance);

            if (Equals(propertyValue, _value))
            {
                if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                    return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName} is required");
            }

            return ValidationResult.Success;
        }
    }
}
