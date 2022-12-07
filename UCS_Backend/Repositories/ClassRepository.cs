using UCS_Backend.Data;
using UCS_Backend.Interfaces.IRepositories;
using UCS_Backend.Models;

namespace UCS_Backend.Repositories
{
    /// <summary>
    /// Creates a class for ClassRepositoty
    /// </summary> 
    public class ClassRepository : IClassRepository
    {
        private DataContext _context;
        /// <summary>
        ///  creates class repo
        /// </summary>
        /// <param name="context"></param>
        public ClassRepository(DataContext context)
        {
            this._context = context;
        }
        /// <summary>
        /// addNew class to class model
        /// </summary>
        /// <param name="classModel"></param>
        /// <returns></returns>
        public int AddNewClass(ClassModel classModel)
        {
            var temp = _context.Classes.Where(x => x.ClssId == classModel.ClssId).FirstOrDefault();
            if (temp != null) // update
            {
                temp.ClssId = classModel.ClssId;
                temp.Enrollments = classModel.Enrollments;
                temp.Course = classModel.Course;
                temp.CourseTitle = classModel.CourseTitle;
                temp.Section = classModel.Section;  
                temp.CatalogNumber = classModel.CatalogNumber;
                _context.SaveChanges();
                return temp.ClassId;
            }
            else // add
            {
                var res = _context.Classes.Add(classModel);
                _context.SaveChanges();
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
            var res = _context.Classes.Where(x => x.CatalogNumber == catalogNumber && x.Section == section).FirstOrDefault();
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
            var classResult = this._context.Classes.Where(c => c.CatalogNumber == courseNumber && c.Section == sectionNumber).FirstOrDefault();  
            if (classResult == null) // Add
            {
                var res = this._context.Classes.Add(new ClassModel
                {
                    CatalogNumber = courseNumber,
                    Section = sectionNumber,
                    Enrollments = Int32.Parse(enrollment),
                    SubjectCode = subjectCode,
                    CourseTitle = courseTitle,
                    ClssId = clssId,
                    Course = subjectCode + " " + courseNumber
                }).Entity;
                this._context.SaveChanges();
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
            var schedule = this._context.Schedules.Where(x => x.ClassId == Int32.Parse(classID)).FirstOrDefault();
            if (schedule != null)
            {
                schedule.IsDeleted = true;
                this._context.SaveChanges();
            }
        }
    }
}
