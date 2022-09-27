using UCS_Backend.Models;

namespace UCS_Backend.Interfaces.IManagers
{
    public interface IScheduleManager
    {
        Schedule AddSchedule(ScheduleFormBody body);
    }
}
