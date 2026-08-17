using Microsoft.EntityFrameworkCore;
using TalentOrbitApi.Models;
using TalentOrbitApi.Models.Entities;

namespace TalentOrbitApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected ApplicationDbContext()
        {
        }

        public DbSet<Mentor> Mentors { get; set; } = null!;
    }
}
