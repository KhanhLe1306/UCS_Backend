using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
using System.Linq.Expressions;
using UCS_Backend.Interfaces;
using UCS_Backend.Interfaces.IManagers;
using UCS_Backend.Interfaces.IRepositories;
using UCS_Backend.Models;
using UCS_Backend.Models.SubModels;
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

        /// <summary>
        /// add class model to class test
        /// </summary>
        /// <param name="addClassModel"></param>
        /// <returns></returns>    
        [HttpPost("addClass")]
        public SuccessInfo addClass(AddClassModel addClassModel)
        {
            var res = this._scheduleRepository.ValidateInsert(addClassModel);
            return res;
        }

        [HttpPost("removeClass/{classID}")]
        public void RemoveClass(string classID)
        {
            this._classRepository.RemoveClass(classID);
        }

        [HttpPost("updateClass")]
        public SuccessInfo UpdateClass([FromBody] UpdateClassModel updateClassModel)
        {
            SuccessInfo successInfo = this._classRepository.ValidateClassUpdate(updateClassModel);
            if (successInfo.success == true)
            {
                int timeID = this._timeRepository.GetTimeId(updateClassModel.StartTime, updateClassModel.EndTime);
                int weekdayID = this._weekdayRepository.GetWeekDaysIdByDescription(updateClassModel.MeetingDays);
                this._scheduleRepository.UpdateClassInSchedule(updateClassModel.ScheduleID, timeID, weekdayID);
            }
            return successInfo;
        }


    }
}
