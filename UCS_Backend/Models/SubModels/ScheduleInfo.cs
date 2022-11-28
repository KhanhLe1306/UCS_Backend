namespace UCS_Backend.Models.SubModels
{
    public class ScheduleInfo
    {
        public string ClssID { get; set; }
        public string RoomName { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Course { get; set; }
        public string CourseTitle { get; set; }
        public string MeetingDays { get; set; }
        public string CrossListedWith { get; set; }
        public string CrossListedClssID { get; set; }
        public string Instructor { get; set; }
        public Dictionary<string, Dictionary<string, string>> PriorCourseInfo { get; set; }
    }
}
