namespace UCS_Backend.Models
{

     /// <summary>
    /// Creates a class for ScheduleFormBody
    /// </summary> 
    public class ScheduleFormBody
    {
        public string RoomName { get; set; }
        public string Days { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }
}
