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
        /// Finds a class given a course number and a section number.
        /// If the class isn't found, a new one is inserted into the database.
        /// </summary>
        /// <param name="courseNumber"> Course number of the class to be searched or added (i.e. 1200)</param>
        /// <param name="sectionNumber">Section number of the class to be searched or added (i.e 001)</param>
        /// <param name="enrollment">Enrollment of the class to be added</param>
        /// <param name="subjectCode">Subject code of class to be added</param>
        /// <param name="courseTitle">Title of class to be added</param>
        /// <param name="clssId">clssId of class to be added (default is 0 for new classes)</param>
        /// <returns>ClassId of the class found or added</returns>
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
        /// Find the assoicated ScheduleId of given the ClssId
        /// </summary>
        /// <param name="clssId">ClssId of the class</param>
        /// <returns>ScheudleId</returns>
        public int GetScheduleIdByClssId(int clssId)
        {
            int classResult = this._dataContext.Classes.Where(c => c.ClssId == clssId).FirstOrDefault().ClassId;
            int ScheduleId = this._dataContext.Schedules.Where(s => s.ClassId == classResult).FirstOrDefault().ScheduleId;
            return ScheduleId;
            
        }


        /// <summary>
        /// Removes a class from the database given a ClassId.
        /// Cross listed classes are also removed.
        /// </summary>
        /// <param name="classID">ClassId of the class to be removed</param>
        public void RemoveClass(string classID)
        {
            var schedule = this._dataContext.Schedules.Where(x => x.ClassId == Int32.Parse(classID) && x.IsDeleted != true).FirstOrDefault();
            if (schedule != null)
            {
                schedule.IsDeleted = true;
                this._dataContext.SaveChanges();
                var deez = this._dataContext.Classes.Where(x => x.ClassId == Int32.Parse(classID)).FirstOrDefault();
                if (deez != null)
                {
                    var deez2 = this._dataContext.Cross.Where(x => x.ClssID2 == deez.ClssId).FirstOrDefault();
                    if (deez2 != null)
                    {
                        var deez3 = this._dataContext.Classes.Where(x => x.ClssId == deez2.ClssID1).FirstOrDefault();
                        if (deez3 != null)
                        {
                            var deez4 = this._dataContext.Schedules.Where(x => x.ClassId == deez3.ClassId).FirstOrDefault();
                            if (deez4 != null)
                            {
                                Console.WriteLine($"THERE IS A CROSS LISITNG THAT NEEDS TO BE DELETED");
                                Console.WriteLine($"The crosslisting is {deez3.Course}");
                                deez4.IsDeleted = true;
                                this._dataContext.SaveChanges();
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Given the form data from the frontend Add Class form,
        /// this function valdiates the class can be added without any
        /// conflicts. If every conflict encountered is marked with an apporiapriate messsage.
        /// </summary>
        /// <param name="updateClassModel">Model representing the AddClass form on the frontend</param>
        /// <returns>SuccessInfo instance telling whether the addition was successful, aong
        /// with a list of messages.</returns>
        public SuccessInfo ValidateClassUpdate(UpdateClassModel updateClassModel)
        {
            bool roomCheck = true;
            bool instructorCheck = true;
            List<Dictionary<string, string>> messages = new List<Dictionary<string, string>>();

            Tuple<int, int> time = Tuple.Create(Int32.Parse(updateClassModel.StartTime), Int32.Parse(updateClassModel.EndTime));
            string firstName = updateClassModel.InstructorName.Split(' ')[0];
            string lastName = updateClassModel.InstructorName.Split(' ')[1];

            // Grab the Crosslisted ClassId so we can ignore it also
            int CrossClassId = 0;
            if (Int32.Parse(updateClassModel.CrossListedClssId) != 0)
            {
                var temp = _dataContext.Classes.Where(x => x.ClssId == int.Parse(updateClassModel.CrossListedClssId));
                foreach (var item in temp)
                {
                    CrossClassId = item.ClassId;
                }
            }

            // ROOM CHECK
            var res = (from r in _dataContext.Rooms
                       join s in _dataContext.Schedules on r.RoomId equals s.RoomId
                       join t in _dataContext.Time on s.TimeId equals t.TimeId
                       join c in _dataContext.Classes on s.ClassId equals c.ClassId
                       join w in _dataContext.Weekdays on s.WeekdayId equals w.WeekdayId
                       join ic in _dataContext.InstructorClasses on s.ClassId equals ic.ClassId
                       join i in _dataContext.Instructors on ic.InstructorId equals i.InstructorId
                       where r.Name == updateClassModel.RoomName && s.IsDeleted != true && c.ClassId != Int32.Parse(updateClassModel.ClassId) && c.ClassId != CrossClassId
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
                   where i.FirstName == firstName && i.LastName == lastName && c.ClassId != Int32.Parse(updateClassModel.ClassId) && c.ClassId != CrossClassId
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
            if (roomCheck && instructorCheck)
            {
                messages.Add(new Dictionary<string, string> { { "header", $"Update Successful" }, { "message-primary", $"{updateClassModel.Course}-{updateClassModel.Section}, {updateClassModel.InstructorName}" }, { "message-secondary", $"Meeting: {time.Item1} - {time.Item2}, {string.Join(" & ", updateClassModel.MeetingDays.Split(','))}" } });
            }

            return new SuccessInfo { success = roomCheck && instructorCheck, messages = messages };
        }
    }
}
