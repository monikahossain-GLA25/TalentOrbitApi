
using Microsoft.EntityFrameworkCore;
using TalentOrbitApi.Data;
using TalentOrbitApi.Models.Dtos;
using TalentOrbitApi.Models.Entities;
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
            // Step 1: Start with a read-only query.
            IQueryable<Mentor> query =
                _context.Mentors.AsNoTracking();

            // Step 2: Apply search.
            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchTerm = search.Trim();

                query = query.Where(mentor =>
                    mentor.FullName.Contains(searchTerm) ||
                    mentor.EmailAddress.Contains(searchTerm) ||
                    (mentor.PhoneNumber != null &&
                     mentor.PhoneNumber.Contains(searchTerm)));
            }

            // Step 3: Count matching records.
            var totalCount =
                await query.CountAsync(cancellationToken);

            // Step 4: Apply sorting.
            var descending = sortDirection == "desc";

            IOrderedQueryable<Mentor> orderedQuery =
                sortBy switch
                {
                    "emailaddress" => descending
                        ? query.OrderByDescending(m => m.EmailAddress)
                        : query.OrderBy(m => m.EmailAddress),

                    "hourlyrate" => descending
                        ? query.OrderByDescending(m => m.HourlyRate)
                        : query.OrderBy(m => m.HourlyRate),

                    _ => descending
                        ? query.OrderByDescending(m => m.FullName)
                        : query.OrderBy(m => m.FullName)
                };

            // Step 5: Apply pagination.
            var items = await orderedQuery
                .ThenBy(m => m.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new MentorDto
                {
                    Id = m.Id,
                    FullName = m.FullName,
                    EmailAddress = m.EmailAddress,
                    PhoneNumber = m.PhoneNumber,
                    HourlyRate = m.HourlyRate
                })
                .ToListAsync(cancellationToken);

            // Step 6: Return paginated data.
            return new PagedResult<MentorDto>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(
                    totalCount / (double)pageSize)
            };
        }
    }
}
