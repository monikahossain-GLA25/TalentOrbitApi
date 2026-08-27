using System.ComponentModel.DataAnnotations;

namespace TalentOrbitApi.Models.Dtos
{
    public class UpdateMentorDto
    {
        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(
            100,
            MinimumLength = 2,
            ErrorMessage = "Full name must contain between 2 and 100 characters.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [StringLength(
            150,
            ErrorMessage = "Email address cannot exceed 150 characters.")]
        public string EmailAddress { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Enter a valid phone number.")]
        [StringLength(
            20,
            ErrorMessage = "Phone number cannot exceed 20 characters.")]
        public string? PhoneNumber { get; set; }

        [Range(
            typeof(decimal),
            "1",
            "1000000",
            ErrorMessage = "Hourly rate must be between 1 and 1,000,000.")]
        public decimal HourlyRate { get; set; }
    }
}