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
            return res != null ? res.ClssId : 0;
        }
    }
}
