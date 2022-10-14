using UCS_Backend.Interfaces;
using UCS_Backend.Interfaces.IManagers;
using UCS_Backend.Models;

namespace UCS_Backend.Managers
{
    public class ScheduleManager : IScheduleManager
    {
        private IScheduleRepository _scheduleRepository;
        public ScheduleManager()
        {

        }
        public Schedule AddSchedule(ScheduleFormBody body)
        {
            throw new NotImplementedException();
        }
    }
}
