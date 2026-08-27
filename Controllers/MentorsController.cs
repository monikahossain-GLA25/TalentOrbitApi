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
        public async Task<ActionResult<List<MentorDto>>>
    GetAllMentors()
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


    }
}
