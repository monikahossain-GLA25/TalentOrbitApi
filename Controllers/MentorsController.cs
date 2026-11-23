
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using TalentOrbitApi.Data;
using TalentOrbitApi.Models.Dtos;
using TalentOrbitApi.Models.Entities;
using TalentOrbitApi.Services.Interfaces;

namespace TalentOrbitApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MentorsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMentorService _mentorService;

        public MentorsController(
            ApplicationDbContext context,
            IMentorService mentorService)
        {
            _context = context;
            _mentorService = mentorService;
        }

        // GET: api/mentors
        // Supports search, sorting and pagination
        [HttpGet]
        public async Task<ActionResult<PagedResult<MentorDto>>> GetAllMentors(
            [FromQuery] string? search,
            [FromQuery] string? sortBy = "fullName",
            [FromQuery] string? sortDirection = "asc",
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            // Step 1: Validate pagination
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

            var offset = ((long)pageNumber - 1) * pageSize;

            if (offset > int.MaxValue)
            {
                return Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Page number is too large",
                    detail: "The requested page exceeds the supported offset.");
            }

            // Step 2: Validate sorting
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

            // Step 3: Build query
            var result = await _mentorService.GetAllAsync(
      pageNumber,
      pageSize,
      search,
      normalizedSortBy,
      normalizedDirection,
      cancellationToken);

            return Ok(result);
        }

        // GET: api/mentors/{id}
        [HttpGet("{id:guid}")]
    
        public async Task<ActionResult<MentorDto>> GetMentorById(
    [FromRoute] Guid id,
    CancellationToken cancellationToken)
        {
            var mentor = await _mentorService.GetByIdAsync(
                id,
                cancellationToken);

            if (mentor is null)
            {
                return NotFound(new
                {
                    message = $"Mentor with ID {id} not found."
                });
            }

            return Ok(mentor);
        }
        // POST: api/mentors
        [HttpPost]
        public async Task<ActionResult<MentorDto>> AddMentor(
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

            await _context.Mentors.AddAsync(mentorEntity);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetMentorById),
                new { id = mentorEntity.Id },
                MapToDto(mentorEntity));
        }

        // PUT: api/mentors/{id}
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<MentorDto>> UpdateMentor(
            [FromRoute] Guid id,
            [FromBody] UpdateMentorDto updateMentorDto)
        {
            var mentor = await _context.Mentors.FindAsync(id);

            if (mentor is null)
            {
                return NotFound(new
                {
                    message = $"Mentor with ID {id} not found."
                });
            }

            mentor.FullName = updateMentorDto.FullName.Trim();
            mentor.EmailAddress = updateMentorDto.EmailAddress.Trim();
            mentor.PhoneNumber = updateMentorDto.PhoneNumber?.Trim();
            mentor.HourlyRate = updateMentorDto.HourlyRate;

            await _context.SaveChangesAsync();

            return Ok(MapToDto(mentor));
        }

        // DELETE: api/mentors/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteMentor(
            [FromRoute] Guid id)
        {
            var mentor = await _context.Mentors.FindAsync(id);

            if (mentor is null)
            {
                return NotFound(new
                {
                    message = $"Mentor with ID {id} was not found."
                });
            }

            _context.Mentors.Remove(mentor);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Convert Mentor entity into MentorDto
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
