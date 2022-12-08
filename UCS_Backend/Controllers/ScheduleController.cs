using Microsoft.AspNetCore.Mvc;
using UCS_Backend.Interfaces;
using UCS_Backend.Interfaces.IManagers;
using UCS_Backend.Interfaces.IRepositories;
using UCS_Backend.Models;
using UCS_Backend.Repositories;
using UCS_Backend.Utils;

namespace UCS_Backend.Controllers
{
    [ApiController]
    [Route("api/{controller}/")]

        /// <summary>
         /// Creates a class for ScheduleController
         /// </summary> 
    public class ScheduleController : Controller
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
        private CSVParser parser;
        /// <summary>
         /// Creates method for controller scheduling
         /// </summary>
         /// <param name="scheduleRepository">creates schedule repo </param>
         /// <param name="scheduleManager">creates schedule manager repo </param>
         /// <param name="classRepository">creates classroom repo </param>
         /// <param name="roomeRepository">creates room repo </param>
         /// <param name="timeRepository">creates time space for repo </param>
         /// <param name="weekdayRepository">creates week days repo </param>
         /// <param name="crossRepository">creates cross listing repo </param>
         /// <param name="instructorRepository">creates instructor repo </param>
         /// <param name="instructorClassRepository">creates instuctors class repo </param>
        public ScheduleController(IScheduleRepository scheduleRepository, IScheduleManager scheduleManager, IClassRepository classRepository, IRoomRepository roomRepository, ITimeRepository timeRepository, IWeekdayRepository weekdayRepository, ICrossRepository crossRepository, IInstructorRepository instructorRepository, IInstructorClassRepository instructorClassRepository)
        {
            this._scheduleRepository = scheduleRepository;
            this._scheduleManager = scheduleManager;
            this._classRepository = classRepository;
            this._instructorRepository = instructorRepository;
            this._instructorClassRepository = instructorClassRepository;
            this._weekdayRepository = weekdayRepository;
            this.parser = new CSVParser(classRepository, roomRepository, timeRepository, weekdayRepository, crossRepository, scheduleRepository, instructorRepository, instructorClassRepository);
        }
        [HttpGet]
        /// <summary>
        /// get all schedules
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Schedule> GetAllSchedules()
        {
            return _scheduleRepository.GetAllSchedules();
        }
         /// <summary>
        /// adds schedule from body
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("add")]
        public Schedule AddSchedule([FromBody] ScheduleFormBody body)
        {
            return this._scheduleManager.AddSchedule(body);
        }
        /// <summary>
        /// creates list for csv file
        /// </summary>
        [HttpGet("parseCSV")]
        public void ParseCSV()
        {
            List<string> csvfiles = new List<string> { "CSCI1191.csv" };
            var processedData = this.parser.processBaseFiles(csvfiles);
            this.parser.process(processedData);
        }

    }
}
