using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TalentOrbitApi.Data;

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
        public IActionResult GetAllMentors()
        {
            var allMentors = applicationDbContext.Mentors.ToList();
            return Ok(allMentors);
        }

        // GET: api/Mentors/{id}
        [HttpGet]
        [Route("{id:guid}")]
        public IActionResult GetMentorById(Guid id)
        {
            var mentor = applicationDbContext.Mentors.Find(id);
            if (mentor == null)
            {
                return NotFound();

            }
            
            return Ok(mentor);
            
        }

   
        
    }
}
