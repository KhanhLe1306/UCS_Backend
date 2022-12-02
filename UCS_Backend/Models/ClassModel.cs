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
        public string CatalogNumber { get; set; }

    }

    public class AddClassModel
    {
        public string cls { get; set; }
        public string section { get; set; }
        public string instructor { get; set; }
        public string classSize { get; set; }
        public string classStart { get; set; }
        public string classEnd { get; set; }
        public string roomCode { get; set; }
        public string room { get; set; }
        public string days { get; set; }
    }
}
