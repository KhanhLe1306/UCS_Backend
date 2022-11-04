using Microsoft.AspNetCore.Mvc;
using UCS_Backend.Interfaces;
using UCS_Backend.Interfaces.IManagers;
using UCS_Backend.Interfaces.IRepositories;
using UCS_Backend.Models;
using UCS_Backend.Utils;

namespace UCS_Backend.Controllers
{
    [ApiController]
    [Route("api/{controller}/")]
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
        private CSVParser parser;

        public ScheduleController(IScheduleRepository scheduleRepository, IScheduleManager scheduleManager, IClassRepository classRepository, IRoomRepository roomRepository, ITimeRepository timeRepository, IWeekdayRepository weekdayRepository, ICrossRepository crossRepository, IInstructorRepository instructorRepository, IInstructorClassRepository instructorClassRepository)
        {
            this._scheduleRepository = scheduleRepository;
            this._scheduleManager = scheduleManager;
            this._classRepository = classRepository;
            this._instructorRepository = instructorRepository;
            this._instructorClassRepository = instructorClassRepository;
            this.parser = new CSVParser(classRepository, roomRepository, timeRepository, weekdayRepository, crossRepository, scheduleRepository, instructorRepository, instructorClassRepository);
        }

        [HttpGet]
        public IEnumerable<Schedule> GetAllSchedules()
        {
            return _scheduleRepository.GetAllSchedules();
        }

        [HttpPost("add")]
        public Schedule AddSchedule([FromBody] ScheduleFormBody body)
        {
            return this._scheduleManager.AddSchedule(body);
        }

        [HttpGet("parseCSV")]
        public void ParseCSV()
        {
            List<string> csvfiles = new List<string> { "CSCI1191.csv" };
            var processedData = this.parser.processBaseFiles(csvfiles);
            this.parser.process(processedData);
        }

    }
}
