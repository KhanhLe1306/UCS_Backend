using System;
namespace UCS_Backend.Models
{

     /// <summary>
    /// Creates a class for Individual
    /// </summary> 
    public class Individual
    {
        public Individual()
        {
        }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int IndividualId { get; set; }
    }
}

