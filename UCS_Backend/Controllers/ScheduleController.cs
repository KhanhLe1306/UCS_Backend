using Microsoft.AspNetCore.Mvc;
using UCS_Backend.Interfaces;
using UCS_Backend.Interfaces.IManagers;
using UCS_Backend.Models;

namespace UCS_Backend.Controllers
{
    [ApiController]
    [Route("api/{controller}")]
    public class ScheduleController : Controller
    {
        private IScheduleRepository _scheduleRepository;
        private IScheduleManager _scheduleManager;

        public ScheduleController(IScheduleRepository scheduleRepository, IScheduleManager scheduleManager)
        {
            this._scheduleRepository = scheduleRepository;
            this._scheduleManager = scheduleManager;
        }

        [HttpGet]
        public IEnumerable<Schedule> GetAllSchedules()
        {
            return _scheduleRepository.GetAllSchedules();
        }

        [HttpPost("/add")]
        public Schedule AddSchedule([FromBody] ScheduleFormBody body)
        {
            return this._scheduleManager.AddSchedule(body);
        }

    }
}
