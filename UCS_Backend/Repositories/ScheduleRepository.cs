using UCS_Backend.Data;
using UCS_Backend.Interfaces;
using UCS_Backend.Models;

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

        public bool ValidateInsert(string cls, string section, string instructor, string classSize, string classTime, string room, string days)
        {
            Tuple<int, int> time = Tuple.Create(Int32.Parse(classTime.Split('-')[0]), Int32.Parse(classTime.Split('-')[1]));
            bool roomCheck = true;
            bool instructorCheck = true;
            if (roomCheck & instructorCheck)
            {
                return true;
            } else
            {
                return false;
            }

        }
    }
}
