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
                            ClssID = c.ClssId.ToString(),
                            RoomName = r.Name,
                            StartTime = t.StartTime.ToString(),
                            EndTime = t.EndTime.ToString(), 
                            Course = c.Course,
                            CourseTitle = c.CourseTitle,
                            MeetingDays = w.Description.ToString(),
                      }).ToList();
                        
            return CheckCrossListedClasses(res);
        }

        public List<ScheduleInfo> CheckCrossListedClasses(List<ScheduleInfo> scheduleInfos)
        {
            var clssIDs = scheduleInfos.Select(r => r.ClssID).ToList();
            clssIDs.Sort();

            var res = (from cross in _dataContext.Cross
                       join a in _dataContext.Classes on cross.ClssID1 equals a.ClssId
                       join b in _dataContext.Classes on cross.ClssID2 equals b.ClssId
                       where cross.ClssID2 != 0 && cross.ClssID2 != cross.ClssID1
                       orderby cross.ClssID1 ascending
                       select new Tuple<string, string, string, string>(b.ClssId.ToString(), b.Course, a.ClssId.ToString(), a.Course)
                       ).ToList();

            foreach(var s in scheduleInfos)
            {
                for(int i = 0; i < res.Count; i++)
                {
                    if(res[i].Item1 == s.ClssID)
                    {
                        s.CrossListedWith = res[i].Item4;
                        s.CrossListedClssID = res[i].Item3;
                        break;
                    }else if(res[i].Item3 == s.ClssID)
                    {
                        s.CrossListedWith = res[i].Item2;
                        s.CrossListedClssID = res[i].Item1;
                        break;
                    }
                }
            }

            Console.WriteLine(scheduleInfos);

        /*    List<string> result = new List<string>();
            foreach(string clssid in clssIDs)
            {
                for(int i = 0; i < res.Count; i++){
                    if(res[i].Item1 == clssid)
                    {
                        result.Add(clssid);
                        clssIDs.Remove(res[i].Item2);   // Remove crosslisted class in the clssIDs
                        // update scheduleInfos
                        scheduleInfos.Where(s => s.ClssID == clssid).FirstOrDefault().CrossListedWith = res[i].Item2;
                        // delete the crosslisted record
                        scheduleInfos.Remove
                        break;
                    }
                }
            }*/

            var crosslistedRecord1 = scheduleInfos.Where(s => s.CrossListedWith != null).ToList();
            var crosslistedRecord2 = crosslistedRecord1;
            var list = new List<ScheduleInfo>();

            var returnResult = scheduleInfos.Where(s => s.CrossListedWith == null).ToList();

            foreach (var temp in crosslistedRecord1)
            {
                for (int i = 0; i < crosslistedRecord2.Count; i++)
                {
                    if (crosslistedRecord2[i].CrossListedClssID == temp.ClssID && Int32.Parse(temp.Course.Substring(5)) < Int32.Parse(crosslistedRecord2[i].Course.Substring(5)))
                    {
                        list.Add(temp);
                        returnResult.Add(temp);
                    }
                }
            }

            return returnResult;
        }

        public void Update(Room entity)
        {
            throw new NotImplementedException();
        }
    }
}
