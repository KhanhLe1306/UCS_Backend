using UCS_Backend.Data;
using UCS_Backend.Interfaces.IRepositories;
using UCS_Backend.Models;
using UCS_Backend.Models.SubModels;

namespace UCS_Backend.Repositories
{
    /// <summary>
    /// Creates a class for ClassRepositoty
    /// </summary> 
    public class ClassRepository : IClassRepository
    {
        private DataContext _dataContext;
        /// <summary>
        ///  creates class repo
        /// </summary>
        /// <param name="context"></param>
        public ClassRepository(DataContext context)
        {
            this._dataContext = context;
        }
        /// <summary>
        /// addNew class to class model
        /// </summary>
        /// <param name="classModel"></param>
        /// <returns></returns>
        public int AddNewClass(ClassModel classModel)
        {
            var temp = _dataContext.Classes.Where(x => x.ClssId == classModel.ClssId).FirstOrDefault();
            if (temp != null) // update
            {
                temp.ClssId = classModel.ClssId;
                temp.Enrollments = classModel.Enrollments;
                temp.Course = classModel.Course;
                temp.CourseTitle = classModel.CourseTitle;
                temp.Section = classModel.Section;  
                temp.CatalogNumber = classModel.CatalogNumber;
                _dataContext.SaveChanges();
                return temp.ClassId;
            }
            else // add
            {
                var res = _dataContext.Classes.Add(classModel);
                _dataContext.SaveChanges();
                return res.Entity.ClassId;
            }              
        }
        /// <summary>
        /// Find class by ID where catalogNumber and section are used
        /// </summary>
        /// <param name="catalogNumber"></param>
        /// <param name="section"></param>
        /// <returns></returns>
        public int FindClssID(string catalogNumber, string section)
        {
            var res = _dataContext.Classes.Where(x => x.CatalogNumber == catalogNumber && x.Section == section).FirstOrDefault();
            if (res != null && res.ClssId != null)
            {
                return (int)res.ClssId;
            }else
            {
                return 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="courseNumber"></param>
        /// <param name="sectionNumber"></param>
        /// <returns></returns>
        public int GetClassIdByCourseAndSection(string courseNumber, string sectionNumber, string enrollment, string subjectCode, string courseTitle, int clssId)
        {
            var classResult = this._dataContext.Classes.Where(c => c.CatalogNumber == courseNumber && c.Section == sectionNumber).FirstOrDefault();  
            if (classResult == null) // Add
            {
                var res = this._dataContext.Classes.Add(new ClassModel
                {
                    CatalogNumber = courseNumber,
                    Section = sectionNumber,
                    Enrollments = Int32.Parse(enrollment),
                    SubjectCode = subjectCode,
                    CourseTitle = courseTitle,
                    ClssId = clssId,
                    Course = subjectCode + " " + courseNumber
                }).Entity;
                this._dataContext.SaveChanges();
                return res.ClassId;
            }
            else
            {
                return classResult.ClassId;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="classID"></param>
        public void RemoveClass(string classID)
        {
            var schedule = this._dataContext.Schedules.Where(x => x.ClassId == Int32.Parse(classID)).FirstOrDefault();
            if (schedule != null)
            {
                schedule.IsDeleted = true;
                this._dataContext.SaveChanges();
            }
        }

        public SuccessInfo ValidateClassUpdate(UpdateClassModel updateClassModel)
        {
            bool roomCheck = true;
            bool instructorCheck = true;
            List<Dictionary<string, string>> messages = new List<Dictionary<string, string>>();

            Tuple<int, int> time = Tuple.Create(Int32.Parse(updateClassModel.StartTime), Int32.Parse(updateClassModel.EndTime));
            string firstName = updateClassModel.InstructorName.Split(' ')[0];
            string lastName = updateClassModel.InstructorName.Split(' ')[1];

            // ROOM CHECK
            var res = (from r in _dataContext.Rooms
                       join s in _dataContext.Schedules on r.RoomId equals s.RoomId
                       join t in _dataContext.Time on s.TimeId equals t.TimeId
                       join c in _dataContext.Classes on s.ClassId equals c.ClassId
                       join w in _dataContext.Weekdays on s.WeekdayId equals w.WeekdayId
                       join ic in _dataContext.InstructorClasses on s.ClassId equals ic.ClassId
                       join i in _dataContext.Instructors on ic.InstructorId equals i.InstructorId
                       where r.Name == updateClassModel.RoomName && s.IsDeleted != true
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
                foreach (var day in updateClassModel.MeetingDays.Split(','))
                {
                    Console.WriteLine(day);
                    if (item.MeetingDays.Contains(day))
                    {
                        if ((time.Item1 >= Int32.Parse(item.StartTime)) & time.Item1 <= Int32.Parse(item.EndTime) | ((time.Item2 >= Int32.Parse(item.StartTime)) & time.Item2 <= Int32.Parse(item.EndTime)))
                        {
                            messages.Add(new Dictionary<string, string> { { "header", "TIME CONFLICT" }, { "message-primary", $"Room {updateClassModel.RoomName} is already booked by {item.Instructor} on {day}" }, { "message-secondary", $"Time: {item.StartTime} - {item.EndTime}" } });
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
                foreach (var day in updateClassModel.MeetingDays.Split(','))
                {
                    if (item.MeetingDays.Contains(day))
                    {
                        if ((time.Item1 >= Int32.Parse(item.StartTime)) & time.Item1 <= Int32.Parse(item.EndTime) | ((time.Item2 >= Int32.Parse(item.StartTime)) & time.Item2 <= Int32.Parse(item.EndTime)))
                        {
                            messages.Add(new Dictionary<string, string> { { "header", "INSTRUCTOR CONFLICT" }, { "message-primary", $"Instructor, {updateClassModel.InstructorName}, is already teaching in Room {item.RoomName}" }, { "message-secondary", $"Time: {item.StartTime} - {item.EndTime}" } });
                            roomCheck = false;
                        }
                    }
                }
            }

            return new SuccessInfo { success = roomCheck && instructorCheck, messages = messages };
        }
    }
}
