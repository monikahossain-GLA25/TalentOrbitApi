
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using TalentOrbitApi.Data;
using TalentOrbitApi.Models.Dtos;
using TalentOrbitApi.Models.Entities;

namespace TalentOrbitApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MentorsController : ControllerBase
    {
        private readonly ApplicationDbContext applicationDbContext;

        public MentorsController(ApplicationDbContext applicationDbContext) {
            this.applicationDbContext = applicationDbContext;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<MentorDto>>> GetAllMentors(
            [FromQuery] string? search,
            [FromQuery] string? sortBy = "fullName",
            [FromQuery] string? sortDirection = "asc",
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            // 1. Validate pagination inputs.
            if (pageNumber < 1)
            {
                return Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Invalid page number",
                    detail: "Page number must be at least 1.");
            }

            if (pageSize < 1 || pageSize > 100)
            {
                return Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Invalid page size",
                    detail: "Page size must be between 1 and 100.");
            }

            // Calculate using long to avoid integer overflow.
            var offset = ((long)pageNumber - 1) * pageSize;

            if (offset > int.MaxValue)
            {
                return Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Page number is too large",
                    detail: "The requested page exceeds the supported offset.");
            }

            // 2. Normalize and validate sorting inputs.
            var normalizedSortBy =
                (sortBy ?? "fullName").Trim().ToLowerInvariant();

            var normalizedDirection =
                (sortDirection ?? "asc").Trim().ToLowerInvariant();

            if (normalizedSortBy is not
                ("fullname" or "emailaddress" or "hourlyrate"))
            {
                return Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Invalid sort field",
                    detail: "Sort by must be fullName, emailAddress, or hourlyRate.");
            }

            if (normalizedDirection is not ("asc" or "desc"))
            {
                return Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Invalid sort direction",
                    detail: "Sort direction must be asc or desc.");
            }

            // 3. Build the query without loading all records.
            IQueryable<Mentor> query =
                applicationDbContext.Mentors.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var searchTerm = search.Trim();

                query = query.Where(mentor =>
                    mentor.FullName.Contains(searchTerm) ||
                    mentor.EmailAddress.Contains(searchTerm) ||
                    (
                        mentor.PhoneNumber != null &&
                        mentor.PhoneNumber.Contains(searchTerm)
                    ));
            }

            // 4. Count all matching records before applying pagination.
            var totalCount = await query.CountAsync(cancellationToken);

            // 5. Apply the selected sorting.
            var descending = normalizedDirection == "desc";

            IOrderedQueryable<Mentor> orderedQuery =
                normalizedSortBy switch
                {
                    "emailaddress" => descending
                        ? query.OrderByDescending(mentor => mentor.EmailAddress)
                        : query.OrderBy(mentor => mentor.EmailAddress),

                    "hourlyrate" => descending
                        ? query.OrderByDescending(mentor => mentor.HourlyRate)
                        : query.OrderBy(mentor => mentor.HourlyRate),

                    _ => descending
                        ? query.OrderByDescending(mentor => mentor.FullName)
                        : query.OrderBy(mentor => mentor.FullName)
                };

            // 6. Fetch only the requested page.
            var items = await orderedQuery
                .ThenBy(mentor => mentor.Id)
                .Skip((int)offset)
                .Take(pageSize)
                .Select(mentor => new MentorDto
                {
                    Id = mentor.Id,
                    FullName = mentor.FullName,
                    EmailAddress = mentor.EmailAddress,
                    PhoneNumber = mentor.PhoneNumber,
                    HourlyRate = mentor.HourlyRate
                })
                .ToListAsync(cancellationToken);

            // 7. Return the items and pagination information.
            var result = new PagedResult<MentorDto>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(
                    totalCount / (double)pageSize)
            };

            return Ok(result);
        }


        [HttpGet("{id:guid}")]
       
        public async Task<ActionResult<MentorDto>> GetMentorById(
    [FromRoute] Guid id)
        {
            var mentor = await applicationDbContext.Mentors
                .AsNoTracking()
                .Where(mentor => mentor.Id == id)
                .Select(mentor => new MentorDto
                {
                    Id = mentor.Id,
                    FullName = mentor.FullName,
                    EmailAddress = mentor.EmailAddress,
                    PhoneNumber = mentor.PhoneNumber,
                    HourlyRate = mentor.HourlyRate
                })
                .FirstOrDefaultAsync();

            if (mentor is null)
            {
                return NotFound(new
                {
                    message = $"Mentor with ID {id} not found."
                });
            }

            return Ok(mentor);
        }

        [HttpPost]
        public async Task<ActionResult<MentorDto>>
    AddMentor(
        [FromBody] AddMentorDto addMentorDto,
        [FromHeader(Name = "X-Correlation-ID")]
        string? correlationId)
        {
            if (!string.IsNullOrWhiteSpace(correlationId))
            {
                Response.Headers["X-Correlation-ID"] =
                    correlationId;
            }

            var mentorEntity = new Mentor
            {
                Id = Guid.NewGuid(),
                FullName = addMentorDto.FullName.Trim(),
                EmailAddress = addMentorDto.EmailAddress.Trim(),
                PhoneNumber = addMentorDto.PhoneNumber?.Trim(),
                HourlyRate = addMentorDto.HourlyRate
            };

            await applicationDbContext.Mentors
                .AddAsync(mentorEntity);

            await applicationDbContext.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetMentorById),
                new { id = mentorEntity.Id },
                MapToDto(mentorEntity));
        }
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<MentorDto>>
    UpdateMentor(
        [FromRoute] Guid id,
        [FromBody] UpdateMentorDto updateMentorDto)
        {
                
        var mentor = await applicationDbContext.Mentors.FindAsync(id);
            if(mentor == null)   {
                return NotFound(new
                {
                    message = $"Mentor with ID {id} not found."
                });
            }
            mentor.FullName = updateMentorDto.FullName.Trim();
            mentor.EmailAddress = updateMentorDto.EmailAddress.Trim();
            mentor.PhoneNumber = updateMentorDto.PhoneNumber?.Trim();
            mentor.HourlyRate = updateMentorDto.HourlyRate;

            await applicationDbContext.SaveChangesAsync();
            return Ok(MapToDto(mentor));
        }


        [HttpDelete("{id:guid}")]
        public async Task<IActionResult>
    DeleteMentor([FromRoute] Guid id)
        {
            var mentor = await applicationDbContext.Mentors
                .FindAsync(id);

            if (mentor is null)
            {
                return NotFound(new
                {
                    message = $"Mentor with ID {id} was not found."
                });
            }

            applicationDbContext.Mentors.Remove(mentor);

            await applicationDbContext.SaveChangesAsync();

            return NoContent();
        }

        private static MentorDto MapToDto(Mentor mentor)
        {
            return new MentorDto
            {
                Id = mentor.Id,
                FullName = mentor.FullName,
                EmailAddress = mentor.EmailAddress,
                PhoneNumber = mentor.PhoneNumber,
                HourlyRate = mentor.HourlyRate
            };
        }
    }
}
