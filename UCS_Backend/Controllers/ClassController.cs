﻿using Microsoft.AspNetCore.Mvc;
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

        [HttpPost("addClass")]
        public void addCourse()
        {
            Console.WriteLine("API ADD CLASS ROUTE WORKS . . . PRESS ENTER");
            Console.ReadLine();
        }
    }
}
