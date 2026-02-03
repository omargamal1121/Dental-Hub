using System.ComponentModel.DataAnnotations;

namespace DentalHub.Application.DTOs.Cases
{
    /// <summary>
    /// DTO for approving/rejecting a case request
    /// </summary>
    public class ApproveCaseRequestDto
    {
        [Required(ErrorMessage = "Request ID is required")]
        public Guid RequestId { get; set; }

        [Required(ErrorMessage = "Doctor ID is required")]
        public Guid DoctorId { get; set; }

        [Required(ErrorMessage = "Approval status is required")]
        public bool IsApproved { get; set; }

        [StringLength(500, MinimumLength = 10, ErrorMessage = "Rejection reason must be between 10 and 500 characters")]
        [RequiredIf(nameof(IsApproved), false, ErrorMessage = "Rejection reason is required when rejecting a request")]
        public string? RejectionReason { get; set; }
    }

    /// <summary>
    /// Custom validation attribute for conditional required fields
    /// </summary>
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
