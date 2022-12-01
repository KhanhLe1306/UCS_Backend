using Microsoft.AspNetCore.Mvc;
using UCS_Backend.Interfaces;
using UCS_Backend.Interfaces.IRepositories;
using UCS_Backend.Models;
using UCS_Backend.Models.SubModels;

namespace UCS_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]/")]

    /// <summary>
    /// Creates a class for InstructorController
    /// </summary> 
    public class InstructorController : Controller
    {
        private IInstructorRepository _instructorRepository;
        public InstructorController(IInstructorRepository instructorRepository)
        {
            _instructorRepository = instructorRepository;
        }
        [HttpPost("add")]
        public Instructor Add(Instructor instructor)
        {
            return _instructorRepository.Add(instructor);
        }

        [HttpGet("getScheduleByInstructor/{employeeNumber}")]
        public List<ScheduleInfo> GetScheduleByInstructor(int employeeNumber)
        {
            var res = _instructorRepository.GetScheduleByInstructor(employeeNumber);
            return res;
        }
    }
}
