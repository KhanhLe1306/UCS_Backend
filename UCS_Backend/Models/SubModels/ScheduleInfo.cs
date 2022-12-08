namespace UCS_Backend.Models.SubModels
{

    /// <summary>
    /// Creates a class for Scheduleinfo
    /// </summary> 
    public class ScheduleInfo
    {
        public string ScheduleID { get; set; }
        public string ClassID { get; set; }
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
        public string Section { get; set; }
        public string CatNumber { get; set; }
        public string SubjectCode { get; set; }
        public Dictionary<string, Dictionary<string, string>> PriorCourseInfo { get; set; }
    }
}
