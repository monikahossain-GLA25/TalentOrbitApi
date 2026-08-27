using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace TalentOrbitApi.Models.Entities
{
    public class Mentor
    {
        public Guid Id { get; set; }

        [MaxLength(100)]
        public required string FullName { get; set; }

        [MaxLength(150)]
        public required string EmailAddress { get; set; }

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [Precision(18, 2)]
        public decimal HourlyRate { get; set; }
    }
}