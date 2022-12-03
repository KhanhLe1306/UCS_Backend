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

/// <summary>
/// creates class for class controller
/// </summary>
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
/// <summary>
/// Creates class controller intakes
/// </summary>
/// <param name="scheduleRepository"></param>
/// <param name="scheduleManager"></param>
/// <param name="classRepository"></param>
/// <param name="roomRepository"></param>
/// <param name="timeRepository"></param>
/// <param name="weekdayRepository"></param>
/// <param name="crossRepository"></param>
/// <param name="instructorRepository"></param>
/// <param name="instructorClassRepository"></param>
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


        [HttpPost("addClass/{cls}&{section}&{instructor}&{classSize}&{classTime}&{roomCode}&{room}&{days}")]

/// <summary>
/// add class method inside of class
/// </summary>
/// <param name="cls"></param>
/// <param name="section"></param>
/// <param name="instructor"></param>
/// <param name="classSize"></param>
/// <param name="classTime"></param>
/// <param name="roomCode"></param>
/// <param name="room"></param>
/// <param name="days"></param>
/// <returns></returns>
        public bool addClass(string cls, string section, string instructor, string classSize, string classTime, string roomCode, string room, string days)
        {
            var valid = this._scheduleRepository.ValidateInsert(cls, section, instructor, classSize, classTime, roomCode, room, days);
            return valid;
        }
/// <summary>
/// add class model to class test
/// </summary>
/// <param name="addClassModel"></param>
/// <returns></returns>
      
        [HttpPost("addClass")]
        public bool AddClassTest([FromBody] AddClassModel addClassModel)
        {
            //var valid = this._scheduleRepository.ValidateInsert();
            return true;
        }

           /// <summary>
             /// Use AddClassTest method to add class model
            /// </summary>
         /// <param name="AddClassModel">Classes added to model</param>
    }
}
