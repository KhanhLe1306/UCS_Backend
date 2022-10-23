using System.ComponentModel.DataAnnotations;

namespace UCS_Backend.Models
{
    public class Cross
    {
        [Key]
        public int CrossId { get; set; }    
        public int ClssID1 { get; set; }
        public int ClssID2 { get; set; }
    }
}
