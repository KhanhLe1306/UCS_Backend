using UCS_Backend.Models;

namespace UCS_Backend.Interfaces
{
    public interface IScheduleRepository : IBaseRepository<Schedule>
    {
        List<Schedule> GetAllSchedules();
    }
}
