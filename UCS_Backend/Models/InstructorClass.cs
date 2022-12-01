using System;
namespace UCS_Backend.Models
{

     /// <summary>
    /// Creates a class for InstructorClass
    /// </summary> 
    public class InstructorClass
    {
        public int InstructorClassId { get; set; }
        public int ClassId { get; set; }
        public int InstructorId { get; set; }
    }
}
