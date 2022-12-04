using UCS_Backend.Interfaces;
using UCS_Backend.Interfaces.IManagers;
using UCS_Backend.Models;

namespace UCS_Backend.Managers
{

        /// <summary>
        /// Creates a class for ScheduleManager
        /// </summary> 
    public class ScheduleManager : IScheduleManager
    {
        private IScheduleRepository _scheduleRepository;
        public ScheduleManager()
        {

        }
        /// <summary>
        /// Adds schedule
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public Schedule AddSchedule(ScheduleFormBody body)
        {
            throw new NotImplementedException();
        }
    }
}
