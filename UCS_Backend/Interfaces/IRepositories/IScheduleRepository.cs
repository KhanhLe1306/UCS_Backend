using UCS_Backend.Models;

namespace UCS_Backend.Interfaces
{
    public interface IScheduleRepository : IBaseRepository<Schedule>
    {
        List<Schedule> GetAllSchedules();
        bool ValidateInsert(string cls, string section, string instructor, string classSize, string classTime, string room, string days);
    }
}
