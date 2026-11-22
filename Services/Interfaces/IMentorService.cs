
using TalentOrbitApi.Models.Dtos;

namespace TalentOrbitApi.Services.Interfaces
{
    public interface IMentorService
    {
        Task<PagedResult<MentorDto>> GetAllAsync(
            int pageNumber,
            int pageSize,
            string? search,
            string? sortBy,
            string? sortDirection,
            CancellationToken cancellationToken = default);
    }
}