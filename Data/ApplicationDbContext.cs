using Microsoft.EntityFrameworkCore;
using TalentOrbitApi.Models.Entities;

namespace TalentOrbitApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Mentor> Mentors { get; set; } = null!;
    }
}