using UCS_Backend.Data;
using UCS_Backend.Interfaces.IRepositories;
using UCS_Backend.Models;
using UCS_Backend.Models.SubModels;

namespace UCS_Backend.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private DataContext _dataContext;
        public RoomRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public IEnumerable<Room> GetAll => throw new NotImplementedException();

        public Room Add(Room room)
        {
            int roomId = FindRoomIdByName(room.Name);
            if (roomId == 0)
            {
                var res = _dataContext.Rooms.Add(room).Entity;
                _dataContext.SaveChanges();
                return res;
            }
            else
            {
                return new Room
                {
                    RoomId = roomId,
                    Name = room.Name,
                    Capacity = room.Capacity
                };
            }         
        }

        public void Delete(Room entity)
        {
            throw new NotImplementedException();
        }

        public Room? FindById(int id)
        {
            var res = from r in _dataContext.Rooms
                      where r.RoomId == id
                      select r;
            return res.FirstOrDefault();
        }

        public int FindRoomIdByName(string name)
        {
            var res = from r in _dataContext.Rooms
                      where r.Name == name
                      select r.RoomId;
            return res.FirstOrDefault();
        }

        public List<ScheduleInfo> GetScheduleByRoomNumber(int roomNumber)
        {
            var res = (from r in _dataContext.Rooms 
                      join s in _dataContext.Schedules on r.RoomId equals s.RoomId
                      join t in _dataContext.Time on s.TimeId equals t.TimeId
                      join c in _dataContext.Classes on s.ClassId equals c.ClassId
                      join w in _dataContext.Weekdays on s.WeekdayId equals w.WeekdayId
                      where r.Name.Contains(roomNumber.ToString())
                      select new ScheduleInfo { 
                            RoomName = r.Name,
                            StartTime = t.StartTime.ToString(),
                            EndTime = t.EndTime.ToString(), 
                            Course = c.Course,
                            CourseTitle = c.CourseTitle,
                            MeetingDays = w.Description.ToString(),
                      }).ToList();
            return res;
        }

        public void Update(Room entity)
        {
            throw new NotImplementedException();
        }
    }
}
