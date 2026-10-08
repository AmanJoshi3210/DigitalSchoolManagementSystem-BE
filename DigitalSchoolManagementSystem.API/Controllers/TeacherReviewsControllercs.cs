using DigitalSchoolManagementSystem.Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalSchoolManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/teacher-reviews")]
    [Authorize]
    public class TeacherReviewsController : Controller
    {

        public TeacherReviewsController()
        {
            
        }
    }
}
