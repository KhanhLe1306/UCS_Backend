using System;
using UCS_Backend.Data;
using UCS_Backend.Interfaces;
using UCS_Backend.Models;
using UCS_Backend.Models.SubModels;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace UCS_Backend.Repositories
{
    public class InstructorRepository: IInstructorRepository, IBaseRepository<Instructor>
    {
        private DataContext _dataContext;
        public InstructorRepository(DataContext dataContext)
        {
            this._dataContext = dataContext;
        }
        public IEnumerable<Instructor> GetAll => this._dataContext.Instructors.AsEnumerable();

        public async Task<List<Instructor>> GetAllInstructors()
        {
            return await this._dataContext.Instructors.ToListAsync();
        }
        public async Task<Instructor?> GetInstructorById(int id)
        {
            var res = from i in this._dataContext.Instructors
                      where i.InstructorId == id
                      select new Instructor
                      {
                          InstructorId = i.InstructorId,
                          FirstName = i.FirstName,
                          LastName = i.LastName,
                      };

            return await res.FirstAsync();
        }

        public async Task<Instructor> AddInstructor(Instructor instructor)
        {
            var res = (await _dataContext.Instructors.AddAsync(instructor)).Entity;
            await _dataContext.SaveChangesAsync();
            return res;
        }

        public Instructor Add(Instructor instructor)
        {
            int instructorId = FindInstructorByName(instructor.FirstName, instructor.LastName);
            Console.WriteLine(instructorId);
            if (instructorId == 0)
            {
                var res = _dataContext.Instructors.Add(instructor).Entity;
                _dataContext.SaveChanges();
                return res;
            } 
            else
            {
                return _dataContext.Instructors.Find(instructorId);
            }
            return null;
        }

        public async Task<(bool, Instructor)> UpdateInstructor(Instructor instructor)
        {
            var temp = await this._dataContext.Instructors.Where(i => i.InstructorId == instructor.InstructorId).FirstAsync();
            if (temp != null)
            {
                temp.FirstName = instructor.FirstName;
                temp.LastName = instructor.LastName;
                await _dataContext.SaveChangesAsync();
                return (true, temp);
            }
            else
            {
                return (false, instructor);
            }
        }
        public async Task<bool> DeleteInstructor(Instructor instructor)
        {
            var res = this._dataContext.Instructors.Remove(instructor).Entity;
            if (res != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
        public Instructor? FindById(int id)
        {
            return _dataContext.Instructors.Find(id);
        }

        public int FindInstructorByName(string firstName, string lastName)
        {
            var res = from r in _dataContext.Instructors
                      where r.FirstName == firstName && r.LastName == lastName
                      select r.InstructorId;
            return res.FirstOrDefault();
        }

        public void Delete(Instructor instructor)
        {
            this._dataContext.Instructors.Remove(instructor);
            this._dataContext.SaveChanges();
        }

        public void Update(Instructor instructor)
        {

        }

        public Task<(bool, Instructor)> UpdateInstrutor(Instructor instructor)
        {
            throw new NotImplementedException();
        }

        public List<ScheduleInfo> GetScheduleByInstructor(string firstName, string lastName)
        {
            var res = (from i in _dataContext.Instructors
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

            if (res.Count == 0)
            {
                return new List<ScheduleInfo> { new ScheduleInfo { } };
            }
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

            foreach (var s in scheduleInfos)
            {
                for (int i = 0; i < res.Count; i++)
                {
                    if (res[i].Item1 == s.ClssID)
                    {
                        s.CrossListedWith = res[i].Item4;
                        s.CrossListedClssID = res[i].Item3;
                        break;
                    }
                    else if (res[i].Item3 == s.ClssID)
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

                List<int> monStartTimes = new List<int>();
                List<int> tueStartTimes = new List<int>();
                List<int> wedStartTimes = new List<int>();
                List<int> thuStartTimes = new List<int>();
                List<int> friStartTimes = new List<int>();

                List<int> monEndTimes = new List<int>();
                List<int> tueEndTimes = new List<int>();
                List<int> wedEndTimes = new List<int>();
                List<int> thuEndTimes = new List<int>();
                List<int> friEndTimes = new List<int>();
                foreach (var item in returnResult)
                {
                    item.PriorCourseInfo = new Dictionary<string, Dictionary<string, string>>();
                    if (item.MeetingDays.Contains("Monday"))
                    {
                        monStartTimes.Add(Int32.Parse(item.StartTime));
                        monEndTimes.Add(Int32.Parse(item.EndTime));
                    }
                    if (item.MeetingDays.Contains("Tuesday"))
                    {
                        tueStartTimes.Add(Int32.Parse(item.StartTime));
                        tueEndTimes.Add(Int32.Parse(item.EndTime));
                    }
                    if (item.MeetingDays.Contains("Wednesday"))
                    {
                        wedStartTimes.Add(Int32.Parse(item.StartTime));
                        wedEndTimes.Add(Int32.Parse(item.EndTime));
                    }
                    if (item.MeetingDays.Contains("Thursday"))
                    {
                        thuStartTimes.Add(Int32.Parse(item.StartTime));
                        thuEndTimes.Add(Int32.Parse(item.EndTime));
                    }
                    if (item.MeetingDays.Contains("Friday"))
                    {
                        friStartTimes.Add(Int32.Parse(item.StartTime));
                        friEndTimes.Add(Int32.Parse(item.EndTime));
                    }
                }

                // Day Start Times
                monStartTimes.Sort();
                tueStartTimes.Sort();
                wedStartTimes.Sort();
                thuStartTimes.Sort();
                friStartTimes.Sort();

                // Day End Times
                monEndTimes.Sort();
                tueEndTimes.Sort();
                wedEndTimes.Sort();
                thuEndTimes.Sort();
                friEndTimes.Sort();

                List<int> times = monStartTimes.Concat(tueStartTimes).Concat(wedStartTimes).Concat(thuStartTimes).Concat(friStartTimes).ToList();
                times.Sort();
                int earliest = times[0];
                double earliest_hour = Math.Floor(Convert.ToDouble(earliest) / 100);
                double earliet_minutes = ((Convert.ToDouble(earliest) / 100) % 1) * 100;
                double pixelRatioTo30 = 1.75;

                foreach (var item in returnResult)
                {
                    // Given the current entry, check it's day, and then check to see where the start time occurs
                    // in the start times list (by index). If the index is 0, then this is the first entry for that day
                    // other wise we need to set the current entrys prev values to be startTime at index - 1, same for end
                    // times. 
                    if (item.MeetingDays.Contains("Monday"))
                    {
                        int timeIndex = monStartTimes.IndexOf(Int32.Parse(item.StartTime));
                        double height = ((((monEndTimes[timeIndex] / 100) - (monStartTimes[timeIndex] / 100)) * 60) + (monEndTimes[timeIndex] % 100) - (monStartTimes[timeIndex] % 100)) * pixelRatioTo30;
                        if (timeIndex == 0)
                        {
                            double marginSize = ((Math.Floor(Double.Parse(item.StartTime) / 100) - earliest_hour) * 60) + ((Convert.ToDouble(item.StartTime) / 100) % 1) * 100 - earliet_minutes;
                            marginSize *= pixelRatioTo30;
                            marginSize += 15;
                            item.PriorCourseInfo.Add("Monday", new Dictionary<string, string> { { "prevStart", "900" }, { "prevEnd", "900" }, { "marginSize", marginSize.ToString() }, { "height", height.ToString() } });
                        }
                        else
                        {
                            double marginSize = (Math.Floor(Double.Parse(item.StartTime) / 100) - Math.Floor(Convert.ToDouble(monEndTimes[timeIndex - 1]) / 100)) * 60;
                            marginSize += (((Double.Parse(item.StartTime) / 100) % 1) * 100) - (((Convert.ToDouble(monEndTimes[timeIndex - 1]) / 100) % 1) * 100);
                            marginSize *= pixelRatioTo30;
                            item.PriorCourseInfo.Add("Monday", new Dictionary<string, string> { { "prevStart", monStartTimes[timeIndex - 1].ToString() }, { "prevEnd", monEndTimes[timeIndex - 1].ToString() }, { "marginSize", marginSize.ToString() }, { "height", height.ToString() } });
                        }
                    }
                    if (item.MeetingDays.Contains("Tuesday"))
                    {
                        int timeIndex = tueStartTimes.IndexOf(Int32.Parse(item.StartTime));
                        double height = ((((tueEndTimes[timeIndex] / 100) - (tueStartTimes[timeIndex] / 100)) * 60) + (tueEndTimes[timeIndex] % 100) - (tueStartTimes[timeIndex] % 100)) * pixelRatioTo30;
                        if (timeIndex == 0)
                        {
                            double marginSize = ((Math.Floor(Double.Parse(item.StartTime) / 100) - earliest_hour) * 60) + ((Convert.ToDouble(item.StartTime) / 100) % 1) * 100 - earliet_minutes;
                            marginSize *= pixelRatioTo30;
                            marginSize += 15;
                            item.PriorCourseInfo.Add("Tuesday", new Dictionary<string, string> { { "prevStart", "900" }, { "prevEnd", "900" }, { "marginSize", marginSize.ToString() }, { "height", height.ToString() } });
                        }
                        else
                        {
                            double marginSize = (Math.Floor(Double.Parse(item.StartTime) / 100) - Math.Floor(Convert.ToDouble(tueEndTimes[timeIndex - 1]) / 100)) * 60;
                            marginSize += (((Double.Parse(item.StartTime) / 100) % 1) * 100) - (((Convert.ToDouble(tueEndTimes[timeIndex - 1]) / 100) % 1) * 100);
                            marginSize *= pixelRatioTo30;
                            item.PriorCourseInfo.Add("Tuesday", new Dictionary<string, string> { { "prevStart", tueStartTimes[timeIndex - 1].ToString() }, { "prevEnd", tueEndTimes[timeIndex - 1].ToString() }, { "marginSize", marginSize.ToString() }, { "height", height.ToString() } });
                        }
                    }
                    if (item.MeetingDays.Contains("Wednesday"))
                    {
                        int timeIndex = wedStartTimes.IndexOf(Int32.Parse(item.StartTime));
                        double height = ((((wedEndTimes[timeIndex] / 100) - (wedStartTimes[timeIndex] / 100)) * 60) + (wedEndTimes[timeIndex] % 100) - (wedStartTimes[timeIndex] % 100)) * pixelRatioTo30;
                        if (timeIndex == 0)
                        {
                            double marginSize = ((Math.Floor(Double.Parse(item.StartTime) / 100) - earliest_hour) * 60) + ((Convert.ToDouble(item.StartTime) / 100) % 1) * 100 - earliet_minutes;
                            marginSize *= pixelRatioTo30;
                            marginSize += 15;
                            item.PriorCourseInfo.Add("Wednesday", new Dictionary<string, string> { { "prevStart", "900" }, { "prevEnd", "900" }, { "marginSize", marginSize.ToString() }, { "height", height.ToString() } });
                        }
                        else
                        {
                            double marginSize = (Math.Floor(Double.Parse(item.StartTime) / 100) - Math.Floor(Convert.ToDouble(wedEndTimes[timeIndex - 1]) / 100)) * 60;
                            marginSize += (((Double.Parse(item.StartTime) / 100) % 1) * 100) - (((Convert.ToDouble(wedEndTimes[timeIndex - 1]) / 100) % 1) * 100);
                            marginSize *= pixelRatioTo30;
                            item.PriorCourseInfo.Add("Wednesday", new Dictionary<string, string> { { "prevStart", wedStartTimes[timeIndex - 1].ToString() }, { "prevEnd", wedEndTimes[timeIndex - 1].ToString() }, { "marginSize", marginSize.ToString() }, { "height", height.ToString() } });
                        }
                    }
                    if (item.MeetingDays.Contains("Thursday"))
                    {
                        int timeIndex = thuStartTimes.IndexOf(Int32.Parse(item.StartTime));
                        double height = ((((thuEndTimes[timeIndex] / 100) - (thuStartTimes[timeIndex] / 100)) * 60) + (thuEndTimes[timeIndex] % 100) - (thuStartTimes[timeIndex] % 100)) * pixelRatioTo30;
                        if (timeIndex == 0)
                        {
                            double marginSize = ((Math.Floor(Double.Parse(item.StartTime) / 100) - earliest_hour) * 60) + ((Convert.ToDouble(item.StartTime) / 100) % 1) * 100 - earliet_minutes;
                            marginSize *= pixelRatioTo30;
                            marginSize += 15;
                            item.PriorCourseInfo.Add("Thursday", new Dictionary<string, string> { { "prevStart", "900" }, { "prevEnd", "900" }, { "marginSize", marginSize.ToString() }, { "height", height.ToString() } });
                        }
                        else
                        {
                            double marginSize = (Math.Floor(Double.Parse(item.StartTime) / 100) - Math.Floor(Convert.ToDouble(thuEndTimes[timeIndex - 1]) / 100)) * 60;
                            marginSize += (((Double.Parse(item.StartTime) / 100) % 1) * 100) - (((Convert.ToDouble(thuEndTimes[timeIndex - 1]) / 100) % 1) * 100);
                            marginSize *= pixelRatioTo30;
                            item.PriorCourseInfo.Add("Thursday", new Dictionary<string, string> { { "prevStart", thuStartTimes[timeIndex - 1].ToString() }, { "prevEnd", thuEndTimes[timeIndex - 1].ToString() }, { "marginSize", marginSize.ToString() }, { "height", height.ToString() } });
                        }
                    }
                    if (item.MeetingDays.Contains("Friday"))
                    {
                        int timeIndex = friStartTimes.IndexOf(Int32.Parse(item.StartTime));
                        double height = ((((friEndTimes[timeIndex] / 100) - (friStartTimes[timeIndex] / 100)) * 60) + (friEndTimes[timeIndex] % 100) - (friStartTimes[timeIndex] % 100)) * pixelRatioTo30;
                        if (timeIndex == 0)
                        {
                            double marginSize = ((Math.Floor(Double.Parse(item.StartTime) / 100) - earliest_hour) * 60) + ((Convert.ToDouble(item.StartTime) / 100) % 1) * 100 - earliet_minutes;
                            marginSize *= pixelRatioTo30;
                            marginSize += 15;
                            item.PriorCourseInfo.Add("Friday", new Dictionary<string, string> { { "prevStart", "900" }, { "prevEnd", "900" }, { "marginSize", marginSize.ToString() }, { "height", height.ToString() } });
                        }
                        else
                        {
                            double marginSize = (Math.Floor(Double.Parse(item.StartTime) / 100) - Math.Floor(Convert.ToDouble(friEndTimes[timeIndex - 1]) / 100)) * 60;
                            marginSize += (((Double.Parse(item.StartTime) / 100) % 1) * 100) - (((Convert.ToDouble(friEndTimes[timeIndex - 1]) / 100) % 1) * 100);
                            marginSize *= pixelRatioTo30;
                            item.PriorCourseInfo.Add("Friday", new Dictionary<string, string> { { "prevStart", friStartTimes[timeIndex - 1].ToString() }, { "prevEnd", friEndTimes[timeIndex - 1].ToString() }, { "marginSize", marginSize.ToString() }, { "height", height.ToString() } });
                        }
                    }
                }
            return returnResult;
        }
    }
}
