using System.ComponentModel.DataAnnotations;

namespace UCS_Backend.Models
{

     /// <summary>
     /// Creates a class for ClassModel
    /// </summary> 
    public class ClassModel
    {
        [Key]
        public int ClassId { get; set; }
        public int ClssId { get; set; } 
        public string Course { get; set; }
        public string CourseTitle { get; set; } 
        public int Enrollments { get; set; }
        public string Section { get; set; }
        public string CatalogNumber { get; set; }

    }
    /// <summary>
    /// creates class to add class model
    /// </summary>

    public class AddClassModel
    {
        public string CourseNumber { get; set; }
        public string SectionNumber { get; set; }
        public string InstructorName { get; set; }
        public string Enrollment { get; set; }
        public string ClassStart { get; set; }
        public string ClassEnd { get; set; }
        public string RoomCode { get; set; }
        public string RoomNumber { get; set; }
        public string Days { get; set; }
    }
}
