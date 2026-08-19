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
       
    }
}
