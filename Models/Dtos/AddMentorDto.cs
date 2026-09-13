using System.ComponentModel.DataAnnotations;

namespace TalentOrbitApi.Models.Dtos
{
    public class AddMentorDto
    {
        [Required(ErrorMessage = "Full name is required.")]
        [MaxLength(
            100,
            ErrorMessage = "Full name cannot exceed 100 characters.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [MaxLength(150)]
        public string EmailAddress { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Enter a valid phone number.")]
        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [Range(
            typeof(decimal),
            "1",
            "1000000",
            ErrorMessage = "Hourly rate must be greater than zero.")]
        public decimal HourlyRate { get; set; }
    }
}