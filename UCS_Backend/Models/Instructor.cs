using System;
namespace UCS_Backend.Models
{

     /// <summary>
    /// Creates a class for Instructor
    /// </summary> 
    public class Instructor
    {
        public Instructor()
        {
        }
        public int InstructorId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmployeeNumber { get; set; }
    }
}
