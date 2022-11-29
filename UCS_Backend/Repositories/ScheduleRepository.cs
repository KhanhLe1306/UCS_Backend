using UCS_Backend.Data;
using UCS_Backend.Interfaces;
using UCS_Backend.Models;
using UCS_Backend.Models.SubModels;

namespace UCS_Backend.Repositories
{
    public class ScheduleRepository : IScheduleRepository
    {
        private DataContext _dataContext;
        public ScheduleRepository(DataContext dataContext)
        {
            this._dataContext = dataContext;
        }

        public IEnumerable<Schedule> GetAll => throw new NotImplementedException();

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

        public void Delete(Schedule entity)
        {
            throw new NotImplementedException();
        }

        public Schedule? FindById(int id)
        {
            throw new NotImplementedException();
        }

        public List<Schedule> GetAllSchedules()
        { 
            var res = this._dataContext.Schedules.ToList();

            return res;
        }

        public void Update(Schedule entity)
        {
            throw new NotImplementedException();
        }

        public bool ValidateInsert(string cls, string section, string instructor, string classSize, string classTime, string roomCode, string room, string days)
        {
            Tuple<int, int> time = Tuple.Create(Int32.Parse(classTime.Split('-')[0]), Int32.Parse(classTime.Split('-')[1]));
            bool roomCheck = true;
            bool instructorCheck = true;
            // ROOM CHECK
            var res = (from r in _dataContext.Rooms
                       join s in _dataContext.Schedules on r.RoomId equals s.RoomId
                       join t in _dataContext.Time on s.TimeId equals t.TimeId
                       join c in _dataContext.Classes on s.ClassId equals c.ClassId
                       join w in _dataContext.Weekdays on s.WeekdayId equals w.WeekdayId
                       where r.Name == roomCode + " " + room
                       select new ScheduleInfo
                       {
                           ClssID = c.ClssId.ToString(),
                           RoomName = r.Name,
                           StartTime = t.StartTime.ToString(),
                           EndTime = t.EndTime.ToString(),
                           Course = c.Course,
                           CourseTitle = c.CourseTitle,
                           MeetingDays = w.Description.ToString(),
                       }).ToList();

            foreach (var item in res)
            {
                foreach (var day in days.Split(','))
                {
                    if (item.MeetingDays.Contains(day))
                    {
                        if ((time.Item1 >= Int32.Parse(item.StartTime)) & time.Item1 <= Int32.Parse(item.EndTime) | ((time.Item2 >= Int32.Parse(item.StartTime)) & time.Item2 <= Int32.Parse(item.EndTime)))
                        {
                            Console.WriteLine("TIME CONFLICT:\n\tROOM IS BOOKED ON " + day + "\n\tDURING THE TIME " + item.StartTime + " : " + item.EndTime);
                            roomCheck = false;
                        }
                    }
                }
            }

            // NEED TO DO INSTRUCTOR

            /*res = (from i in _dataContext.Instructors
                       join ic in _dataContext.InstructorClasses on i.InstructorId equals ic.InstructorId
                       join s in _dataContext.Schedules on ic.ClassId equals s.ClassId
                       join t in _dataContext.Time on s.TimeId equals t.TimeId
                       join c in _dataContext.Classes on s.ClassId equals c.ClassId
                       join w in _dataContext.Weekdays on s.WeekdayId equals w.WeekdayId
                       join r in _dataContext.Rooms on s.RoomId equals r.RoomId
                       where i.EmployeeNumber == instructor
                       select new ScheduleInfo
                       {
                           ClssID = c.ClssId.ToString(),
                           RoomName = r.Name,
                           StartTime = t.StartTime.ToString(),
                           EndTime = t.EndTime.ToString(),
                           Course = c.Course,
                           CourseTitle = c.CourseTitle,
                           MeetingDays = w.Description.ToString(),
                       }).ToList();*/
            Console.ReadLine();

            // INSTRUCTOR CHECK
            if (!roomCheck)
                return false;
            if (!instructorCheck)
                return false;
            return true;

        }
    }
}
