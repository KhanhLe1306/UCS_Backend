namespace UCS_Backend.Models
{

     /// <summary>
    /// Creates a class for Schedule
    /// </summary> 
    public class Schedule
    {
        public int ScheduleId { get; set; }
        public int RoomId { get; set; }
        public int TimeId { get; set; }
        public int ClassId { get; set; }
        public int WeekdayId { get; set; }
        public string? Description { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
