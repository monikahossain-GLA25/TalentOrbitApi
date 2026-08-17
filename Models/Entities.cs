namespace TalentOrbitApi.Models.Entities
{
    public class Mentor
    {
        public Guid Id { get; set; }

        public required string FullName { get; set; }

        public required string EmailAddress { get; set; }

        public string? PhoneNumber { get; set; }

        public decimal HourlyRate { get; set; }
    }
}