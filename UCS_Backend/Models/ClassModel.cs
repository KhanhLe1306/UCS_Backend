using System.ComponentModel.DataAnnotations;

namespace UCS_Backend.Models
{
    public class ClassModel
    {
        [Key]
        public int ClassId { get; set; }
        public int ClssId { get; set; } 
        public string Course { get; set; }
        public string CourseTitle { get; set; } 
        public int Enrollments { get; set; }
        public string Section { get; set; }
    }
}
