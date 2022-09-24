using Microsoft.AspNetCore.Mvc;
using UCS_Backend.Interfaces;
using UCS_Backend.Models;

namespace UCS_Backend.Controllers
{
    [ApiController]
    [Route("api/{controller}")]
    public class ScheduleController : Controller
    {
        private IScheduleRepository _scheduleRepository;

        public ScheduleController(IScheduleRepository scheduleRepository)
        {
            this._scheduleRepository = scheduleRepository;  
        }

        [HttpGet]
        public IEnumerable<Schedule> GetAllSchedules()
        {
            return _scheduleRepository.GetAllSchedules();
        }
    }
}
