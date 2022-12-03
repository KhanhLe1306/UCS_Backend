using UCS_Backend.Models;

namespace UCS_Backend.Interfaces.IManagers
{
    /// <summary>
    /// creates class for schedule manager
    /// </summary>
    public interface IScheduleManager
    {
        Schedule AddSchedule(ScheduleFormBody body);
    }
}
