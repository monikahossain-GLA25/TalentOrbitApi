namespace TalentOrbitApi.Models.Dtos
{
    public class MentorDto
    {
        public Guid Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string EmailAddress { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }

        public decimal HourlyRate { get; set; }
    }
}