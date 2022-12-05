using UCS_Backend.Data;
using UCS_Backend.Interfaces;
using UCS_Backend.Interfaces.IRepositories;
using UCS_Backend.Models;
using UCS_Backend.Models.SubModels;

namespace UCS_Backend.Repositories
{
        /// <summary>
        /// Creates a class for ScheduleRepositoty
        /// </summary> 
    public class ScheduleRepository : IScheduleRepository
    {
        private DataContext _dataContext;
        private IRoomRepository _roomRepository;
        private ITimeRepository _timeRepository;
        private IInstructorRepository _instructorRepository;
        private IClassRepository _classRepository;
        public ScheduleRepository(DataContext dataContext, IRoomRepository roomRepository, ITimeRepository timeRepository, IInstructorRepository instructorRepository, IClassRepository classRepository)
        {
            this._dataContext = dataContext;
            this._roomRepository = roomRepository;
            this._timeRepository = timeRepository;
            this._instructorRepository = instructorRepository;
            this._classRepository = classRepository;    
        }

        public IEnumerable<Schedule> GetAll => throw new NotImplementedException();
        /// <summary>
        /// add scheduled added
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public Schedule Add(Schedule s)
        {
            var temp = _dataContext.Schedules.Where(x => x.ClassId == s.ClassId && x.RoomId == s.RoomId && x.TimeId == s.TimeId && x.WeekdayId == s.WeekdayId).FirstOrDefault();
            if (temp == null)
            {
                var res = _dataContext.Schedules.Add(s).Entity;
                _dataContext.SaveChanges();
                return res;
            }
            else
            {
                return temp;
            }
        }

        /// <summary>
        /// delete schedule added
        /// </summary>
        /// <param name="entity"></param>
        public void Delete(Schedule entity)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// finds schedule by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Schedule? FindById(int id)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// puts schedule into a list format
        /// </summary>
        /// <returns></returns>
        public List<Schedule> GetAllSchedules()
        { 
            var res = this._dataContext.Schedules.ToList();

            return res;
        }
        /// <summary>
        /// updates schedule sheet
        /// </summary>
        /// <param name="entity"></param>
        public void Update(Schedule entity)
        {
            throw new NotImplementedException();
        }
        
        public SuccessInfo ValidateInsert(AddClassModel addClassModel)
        {
            bool roomCheck = true;
            bool instructorCheck = true;
            List<Dictionary<string, string>> messages = new List<Dictionary<string, string>>();

            string courseNumber = addClassModel.CourseNumber;
            string section = addClassModel.SectionNumber;
            string classSize = addClassModel.Enrollment;
            string classStart = addClassModel.ClassStart;
            string classEnd = addClassModel.ClassEnd;
            string roomCode = addClassModel.RoomCode;
            string room = addClassModel.RoomNumber;
            string instructor = addClassModel.InstructorName;
            string days = addClassModel.Days;

            Tuple<int, int> time = Tuple.Create(Int32.Parse(classStart), Int32.Parse(classEnd));
            string firstName = instructor.Split(' ')[0];
            string lastName = instructor.Split(' ')[1];

            // ROOM CHECK
            var res = (from r in _dataContext.Rooms
                       join s in _dataContext.Schedules on r.RoomId equals s.RoomId
                       join t in _dataContext.Time on s.TimeId equals t.TimeId
                       join c in _dataContext.Classes on s.ClassId equals c.ClassId
                       join w in _dataContext.Weekdays on s.WeekdayId equals w.WeekdayId
                       join ic in _dataContext.InstructorClasses on s.ClassId equals ic.ClassId
                       join i in _dataContext.Instructors on ic.InstructorId equals i.InstructorId
                       where r.Name.Substring(3, r.Name.Length - 3).Contains(room.ToString()) & room.ToString().Length == 3
                       select new ScheduleInfo
                       {
                           ClssID = c.ClssId.ToString(),
                           RoomName = r.Name,
                           StartTime = t.StartTime.ToString().PadLeft(4, '0'),
                           EndTime = t.EndTime.ToString().PadLeft(4, '0'),
                           Course = c.Course,
                           CourseTitle = c.CourseTitle,
                           MeetingDays = w.Description.ToString(),
                           Instructor = i.FirstName + " " + i.LastName
                       }).ToList();

            foreach (var item in res)
            {
                foreach (var day in days.Split(','))
                {
                    Console.WriteLine(day);
                    if (item.MeetingDays.Contains(day))
                    {
                        if ((time.Item1 >= Int32.Parse(item.StartTime)) & time.Item1 <= Int32.Parse(item.EndTime) | ((time.Item2 >= Int32.Parse(item.StartTime)) & time.Item2 <= Int32.Parse(item.EndTime)))
                        {
                            messages.Add(new Dictionary<string, string> { { "header", "TIME CONFLICT" }, { "message-primary", $"Room {room} is already booked by {item.Instructor} on {day}" }, { "message-secondary", $"Time: {item.StartTime} - {item.EndTime}" } } );
                            roomCheck = false;
                        }
                    }
                }
            }

            // INSTRUCTOR CHECK
            res = (from i in _dataContext.Instructors
                       join ic in _dataContext.InstructorClasses on i.InstructorId equals ic.InstructorId
                       join s in _dataContext.Schedules on ic.ClassId equals s.ClassId
                       join t in _dataContext.Time on s.TimeId equals t.TimeId
                       join c in _dataContext.Classes on s.ClassId equals c.ClassId
                       join w in _dataContext.Weekdays on s.WeekdayId equals w.WeekdayId
                       join r in _dataContext.Rooms on s.RoomId equals r.RoomId
                       where i.FirstName == firstName && i.LastName == lastName
                       select new ScheduleInfo
                       {
                           ClssID = c.ClssId.ToString(),
                           RoomName = r.Name,
                           StartTime = t.StartTime.ToString(),
                           EndTime = t.EndTime.ToString(),
                           Course = c.Course,
                           CourseTitle = c.CourseTitle,
                           MeetingDays = w.Description.ToString(),
                           Instructor = i.FirstName + " " + i.LastName
                       }).ToList();

            foreach (var item in res)
            {
                foreach (var day in days.Split(','))
                {
                    if (item.MeetingDays.Contains(day))
                    {
                        if ((time.Item1 >= Int32.Parse(item.StartTime)) & time.Item1 <= Int32.Parse(item.EndTime) | ((time.Item2 >= Int32.Parse(item.StartTime)) & time.Item2 <= Int32.Parse(item.EndTime)))
                        {
                            messages.Add(new Dictionary<string, string> { { "header", "INSTRUCTOR CONFLICT" }, { "message-primary", $"Instructor, {instructor}, is already teaching in Room {item.RoomName}" }, { "message-secondary", $"Time: {item.StartTime} - {item.EndTime}" } });
                            roomCheck = false;
                        }
                    }
                }
            }

            // Number of Enrollment check with classsize
            bool classSizeCheck = true;
            bool doesRoomExist = true;
            var temp = this._dataContext.Rooms.Where(r => r.Name == $@"{roomCode} {room}").FirstOrDefault();
            if (temp == null)
            {
                doesRoomExist = false;
                messages.Add(new Dictionary<string, string> { { "header", "ROOM CONFLICT" }, { "message-primary", $"Room {roomCode} {room} does not exist"} });
            }else if (temp.Capacity < Int32.Parse(classSize))
            {
                classSizeCheck = false;
                messages.Add(new Dictionary<string, string> { { "header", "CLASS SIZE CONFLICT" }, { "message-primary", $"Room {roomCode} {room} has capacity of {temp.Capacity}" }, { "message-secondary", $"Inserted {classSize}" } });
            }



            if (roomCheck)
            {
                if (instructorCheck & classSizeCheck & doesRoomExist)
                {
                    messages.Add(new Dictionary<string, string> { { "header", $"{instructor}" }, { "message-primary", $"{roomCode + ' ' + room}" }, { "message-secondary", $"{time.Item1} - {time.Item2},{string.Join(' ', days.Split(','))}" } });
                }
            }
            return new SuccessInfo { success = roomCheck & instructorCheck & classSizeCheck & doesRoomExist, messages = messages };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addClassModel"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool AddClass(AddClassModel addClassModel)
        {
            int roomId = this._roomRepository.GetRoomIdByRoomName(addClassModel.RoomCode, addClassModel.RoomNumber);
            int timeId = this._timeRepository.GetTimeId(addClassModel.ClassStart, addClassModel.ClassEnd);
            int instructorId = this._instructorRepository.GetInstuctorId(addClassModel.InstructorName);
            int classId = this._classRepository.GetClassIdByCourseAndSection(addClassModel.CourseNumber, addClassModel.SectionNumber, addClassModel.Enrollment);

            return true;
        }

    }
}
