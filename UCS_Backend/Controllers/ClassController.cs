using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using UCS_Backend.Interfaces;
using UCS_Backend.Interfaces.IManagers;
using UCS_Backend.Interfaces.IRepositories;
using UCS_Backend.Models;
using UCS_Backend.Utils;

namespace UCS_Backend.Controllers
{
    [ApiController]
    [Route("api/{controller}/")]
    public class ClassController : Controller
    {
        private IScheduleRepository _scheduleRepository;
        private IScheduleManager _scheduleManager;
        private IClassRepository _classRepository;
        private IRoomRepository _roomRepository;
        private ITimeRepository _timeRepository;
        private ICrossRepository _crossRepository;
        private IInstructorRepository _instructorRepository;
        private IInstructorClassRepository _instructorClassRepository;
        private IWeekdayRepository _weekdayRepository;

        public ClassController(IScheduleRepository scheduleRepository, IScheduleManager scheduleManager, IClassRepository classRepository, IRoomRepository roomRepository, ITimeRepository timeRepository, IWeekdayRepository weekdayRepository, ICrossRepository crossRepository, IInstructorRepository instructorRepository, IInstructorClassRepository instructorClassRepository)
        {
            this._scheduleRepository = scheduleRepository;
            this._scheduleManager = scheduleManager;
            this._classRepository = classRepository;
            this._instructorRepository = instructorRepository;
            this._instructorClassRepository = instructorClassRepository;
            this._timeRepository = timeRepository;
            this._weekdayRepository = weekdayRepository;
        }

        [HttpPost("addClass/{cls}&{section}&{instructor}&{classSize}&{classTime}&{room}&{days}")]
        public bool addClass(string cls, string section, string instructor, string classSize, string classTime, string room, string days)
        {
            var valid = this._scheduleRepository.ValidateInsert(cls, section, instructor, classSize, classTime, room, days);
            return true;
        }
    }
}
