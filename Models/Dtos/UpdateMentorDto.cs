namespace TalentOrbitApi.Models.Dtos
{
    public class UpdateMnentorDto
    {
        public string FullName { get; set; } = string.Empty;

        public string EmailAddress { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }

        public decimal HourlyRate { get; set; }
    }
}
