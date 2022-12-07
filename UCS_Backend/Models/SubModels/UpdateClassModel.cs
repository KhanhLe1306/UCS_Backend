namespace UCS_Backend.Models.SubModels
{
    public class UpdateClassModel
    {
        public string ScheduleID { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string MeetingDays { get; set; }
        public string RoomName { get; set; }
        public string InstructorName { get; set; }
    }
}
