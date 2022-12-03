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
       /// <summary>
       /// creates instrcutor repo
       /// </summary>
        private IInstructorRepository _instructorRepository;
        public InstructorController(IInstructorRepository instructorRepository)
        {
            _instructorRepository = instructorRepository;
        }
        [HttpPost("add")]
        /// <summary>
        /// adds instructor by name
        /// </summary>
        /// <param name="instructor"></param>
        /// <returns></returns>
        public Instructor Add(Instructor instructor)
        {
            return _instructorRepository.Add(instructor);
        }
        [HttpGet("getScheduleByInstructor/{employeeNumber}")]
        /// <summary>
        /// adds instructor to lst by employee number
        /// </summary>
        /// <param name="employeeNumber"></param>
        /// <returns></returns>
        public List<ScheduleInfo> GetScheduleByInstructor(int employeeNumber)
        {
            var res = _instructorRepository.GetScheduleByInstructor(employeeNumber);
            return res;
        }
    }
}
