using UCS_Backend.Data;
using UCS_Backend.Interfaces;
using UCS_Backend.Interfaces.IRepositories;
using UCS_Backend.Models;
using UCS_Backend.Models.SubModels;

namespace UCS_Backend.Repositories
{
    /// <summary>
    /// Creates a class for ScheduleRepositoty.
    /// </summary> 
    public class ScheduleRepository : IScheduleRepository
    {
        private DataContext _dataContext;
        private IRoomRepository _roomRepository;
        private ITimeRepository _timeRepository;
        private IInstructorRepository _instructorRepository;
        private IClassRepository _classRepository;
        private IWeekdayRepository _weekdayRepository;
        private IInstructorClassRepository _instructorClassRepository;
        public ScheduleRepository(DataContext dataContext, IRoomRepository roomRepository, ITimeRepository timeRepository, IInstructorRepository instructorRepository, IClassRepository classRepository, IWeekdayRepository weekdayRepository, IInstructorClassRepository instructorClassRepository)
        {
            this._dataContext = dataContext;
            this._roomRepository = roomRepository;
            this._timeRepository = timeRepository;
            this._instructorRepository = instructorRepository;
            this._classRepository = classRepository;    
            this._weekdayRepository = weekdayRepository;
            this._instructorClassRepository = instructorClassRepository;
        }

        /// <summary>
        /// Grabs all Schedule Table entries.
        /// </summary>
        public IEnumerable<Schedule> GetAll => throw new NotImplementedException();
        
        /// <summary>
        /// Given a Schedule Object, an entry is inserted into the Schedule Table.
        /// </summary>
        /// <param name="schedule">Instance of the Schedule Class</param>
        /// <returns>The inserted entry if it doesn't already exist, otherwise the entry that already exists</returns>
        public Schedule Add(Schedule schedule)
        {
            var temp = _dataContext.Schedules.Where(x => x.ClassId == schedule.ClassId && x.RoomId == schedule.RoomId && x.TimeId == schedule.TimeId && x.WeekdayId == schedule.WeekdayId).FirstOrDefault();
            if (temp == null)
            {
                var res = _dataContext.Schedules.Add(schedule).Entity;
                _dataContext.SaveChanges();
                return res;
            }
            else
            {
                return temp;
            }
        }

        /// <summary>
        /// Given a Schedule Object, the record is removed from the Schedule Table given it exists.
        /// </summary>
        /// <param name="schedule">Instance of the Schedule Class</param>
        public void Delete(Schedule schedule)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Given an id, return the matching row in the Schedule Table.
        /// </summary>
        /// <param name="id">Integer value mapping to a row in the Schedule Table</param>
        /// <returns></returns>
        public Schedule? FindById(int id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// R
        /// </summary>
        /// <returns></returns>
        public List<Schedule> GetAllSchedules()
        {
            var res = this._dataContext.Schedules.Where(x => x.IsDeleted == false || x.IsDeleted == null).ToList();
            return res;
        }

        /// <summary>
        /// Given a Schedule Object, the function updates that entry in the Schedule Table
        /// </summary>
        /// <param name="entity"></param>
        public void Update(Schedule entity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Given the parameters from the frontend add-class form, different checks are made to ensure the
        /// class can be added to the schedule without conflict. Appropriate messages are logged when conflicts
        /// are encountered. If all checks are passed a call is made to add the class to the Schedule Table.
        /// </summary>
        /// <param name="addClassModel">Model that maps to the inputs from the Add Class form on the front end</param>
        /// <returns>Success Information Model</returns>
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
            string subjectCode = addClassModel.SubjectCode;
            string courseTitle = addClassModel.CourseTitle;
            string sectionNumber = addClassModel.SectionNumber;

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
                           Instructor = i.FirstName + " " + i.LastName,
                           Section = c.Section,
                           CatNumber = c.CatalogNumber,
                           SubjectCode = c.SubjectCode,

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

            // Does this class alredy exist (etchris will add this to fix add class bug)
            bool doesClassNotExist = true;
            var temp2 = this._dataContext.Classes.Where(x => x.SubjectCode == subjectCode && x.CatalogNumber == courseNumber && x.Section == sectionNumber).FirstOrDefault();
            if (temp2 != null)
            {
                doesClassNotExist = false;
                messages.Add(new Dictionary<string, string> { { "header", "CLASS EXISTS CONFLICT" }, { "message-primary", $"Class {subjectCode} {courseNumber} - {courseTitle}" }, {"message-secondary", $"The section {section} already exists!" } });
            }


            if (roomCheck && instructorCheck && classSizeCheck && doesRoomExist && doesClassNotExist)
            {
                // Call add class when all checks are passed
                AddClass(addClassModel);
                messages.Add(new Dictionary<string, string> { { "header", $"{instructor}" }, { "message-primary", $"{roomCode + ' ' + room}" }, { "message-secondary", $"{time.Item1} - {time.Item2},{string.Join(' ', days.Split(','))}" } });
            }

            Console.WriteLine("Printing out errors . . .");
            foreach (Dictionary<string, string> message in messages)
            {
                Console.WriteLine(message["message-primary"]);
            }

            return new SuccessInfo { success = roomCheck && instructorCheck && classSizeCheck && doesRoomExist && doesClassNotExist, messages = messages };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addClassModel">Model that maps to the inputs from the Add Class form on the front end</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public void AddClass(AddClassModel addClassModel)
        {
            int roomId = this._roomRepository.GetRoomIdByRoomName(addClassModel.RoomCode, addClassModel.RoomNumber);
            int timeId = this._timeRepository.GetTimeId(addClassModel.ClassStart, addClassModel.ClassEnd);
            int instructorId = this._instructorRepository.GetInstuctorId(addClassModel.InstructorName);

            // Insert into the class Table
            int classId = this._classRepository.GetClassIdByCourseAndSection(addClassModel.CourseNumber, addClassModel.SectionNumber, addClassModel.Enrollment, addClassModel.SubjectCode, addClassModel.CourseTitle, 0);
            int weekdayId = this._weekdayRepository.GetWeekDaysIdByDescription(addClassModel.Days);

            Console.WriteLine(roomId + " " + timeId + " " + instructorId + " " + classId + " " + weekdayId);
            // Insert into InstructorClass table
            var temp1 = this._instructorClassRepository.AddUpdateInstructorClass(new InstructorClass { 
                ClassId = classId,
                InstructorId = instructorId
            });

            // Insert into Schedule table
            var temp3 = AddUpdateSchedule(new Schedule { 
                ClassId = classId,
                RoomId = roomId,
                TimeId = timeId,
                WeekdayId = weekdayId
            });
        }

        /// <summary>
        /// Takes in a schedule object and attempts to insert into the Schedule Table if 
        /// an entry doesn't already exist
        /// </summary>
        /// <param name="schedule">Instance of the Schedule Class</param>
        /// <returns></returns>
        public Schedule AddUpdateSchedule(Schedule schedule)
        {
            var temp = this._dataContext.Schedules.Where(x => x.ClassId == schedule.ClassId && x.RoomId == schedule.RoomId && x.TimeId == schedule.TimeId && x.WeekdayId == schedule.WeekdayId).FirstOrDefault();
            if (temp != null)
            {
                return temp;
            }
            else
            {
                var res = this._dataContext.Schedules.Add(schedule).Entity;
                this._dataContext.SaveChanges();
                return res;
            }
        }
    }
}
