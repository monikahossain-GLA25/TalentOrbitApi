using Microsoft.AspNetCore.Http;
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
        public async Task<ActionResult<List<MentorDto>>> GetAllMentors()
        {
            var mentors = await applicationDbContext.Mentors
                .AsNoTracking()
                .Select(mentor => new MentorDto
                {
                    Id = mentor.Id,
                    FullName = mentor.FullName,
                    EmailAddress = mentor.EmailAddress,
                    PhoneNumber = mentor.PhoneNumber,
                    HourlyRate = mentor.HourlyRate
                })
                .ToListAsync();

            return Ok(mentors);
        }
        [HttpPost]
        [HttpGet]
        public async Task<ActionResult<List<MentorDto>>>
    GetAllMentors(
        [FromQuery] string? search,
        [FromQuery] string sortBy = "fullName",
        [FromQuery] string sortDirection = "asc")
        {
            var normalizedSortBy =
                sortBy.Trim().ToLowerInvariant();

            var normalizedDirection =
                sortDirection.Trim().ToLowerInvariant();

            if (normalizedSortBy is not
                ("fullname" or "emailaddress" or "hourlyrate"))
            {
                return BadRequest(new
                {
                    message =
                        "Sort by must be fullName, emailAddress, or hourlyRate."
                });
            }

            if (normalizedDirection is not ("asc" or "desc"))
            {
                return BadRequest(new
                {
                    message =
                        "Sort direction must be either 'asc' or 'desc'."
                });
            }

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

            var descending = normalizedDirection == "desc";

            query = normalizedSortBy switch
            {
                "emailaddress" => descending
                    ? query.OrderByDescending(mentor =>
                        mentor.EmailAddress)
                    : query.OrderBy(mentor =>
                        mentor.EmailAddress),

                "hourlyrate" => descending
                    ? query.OrderByDescending(mentor =>
                        mentor.HourlyRate)
                    : query.OrderBy(mentor =>
                        mentor.HourlyRate),

                _ => descending
                    ? query.OrderByDescending(mentor =>
                        mentor.FullName)
                    : query.OrderBy(mentor =>
                        mentor.FullName)
            };

            var mentors = await query
                .Select(mentor => new MentorDto
                {
                    Id = mentor.Id,
                    FullName = mentor.FullName,
                    EmailAddress = mentor.EmailAddress,
                    PhoneNumber = mentor.PhoneNumber,
                    HourlyRate = mentor.HourlyRate
                })
                .ToListAsync();

            return Ok(mentors);
        }
    }
}
