using TalentOrbitApi.Data;
using TalentOrbitApi.DTOs;
using TalentOrbitApi.Models.Dtos;
using TalentOrbitApi.Services.Interfaces;

namespace TalentOrbitApi.Services
{
    public class MentorService : IMentorService
    {
        private readonly ApplicationDbContext _context;

        public MentorService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<MentorDto>> GetAllAsync(
            int pageNumber,
            int pageSize,
            string? search,
            string? sortBy,
            string? sortDirection,
            CancellationToken cancellationToken = default)
        {
            
            throw new NotImplementedException();
        }
    }
}