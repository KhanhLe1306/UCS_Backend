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
        public string Course { get; set; }
        public string CrossListedClssId { get; set; }
        public string Section { get; set; }
        public string ClassId { get; set; }
    }
}
