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
        public string Cls { get; set; }
        public string Section { get; set; }
        public string Instructor { get; set; }
        public string ClassSize { get; set; }
        public string ClassTime { get; set; }
        public string RoomCode { get; set; }
        public string Room { get; set; }
        public string Days { get; set; }
    }
}
