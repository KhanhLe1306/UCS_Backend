using UCS_Backend.Data;
using UCS_Backend.Interfaces.IRepositories;
using UCS_Backend.Models;

namespace UCS_Backend.Repositories
{
    public class ClassRepository : IClassRepository
    {
        private DataContext _context;
        public ClassRepository(DataContext context)
        {
            this._context = context;
        }
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
                temp.Instructor = classModel.Instructor;
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

        public int FindClssID(string catalogNumber, string section)
        {
            var res = _context.Classes.Where(x => x.CatalogNumber == catalogNumber && x.Section == section).FirstOrDefault();
            return res != null ? res.ClssId : 0;
        }
    }
}
