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

        public List<ScheduleInfo> GetScheduleByInstructor(int employeeNumber)
        {
            var res = (from i in _dataContext.Instructors
                       join ic in _dataContext.InstructorClasses on i.InstructorId equals ic.InstructorId
                       join s in _dataContext.Schedules on ic.ClassId equals s.ClassId
                       join t in _dataContext.Time on s.TimeId equals t.TimeId
                       join c in _dataContext.Classes on s.ClassId equals c.ClassId
                       join w in _dataContext.Weekdays on s.WeekdayId equals w.WeekdayId
                       join r in _dataContext.Rooms on s.RoomId equals r.RoomId
                       where i.EmployeeNumber == employeeNumber.ToString()
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

            return returnResult;
        }
    }
}
