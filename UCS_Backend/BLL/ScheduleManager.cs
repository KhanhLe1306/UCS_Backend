using UCS_Backend.Interfaces;
using UCS_Backend.Interfaces.IManagers;
using UCS_Backend.Interfaces.IRepositories;
using UCS_Backend.Models;


namespace UCS_Backend.BLL
{

    /// <summary>
    /// Creates a class for ScheduleManager
    /// </summary>  
    public class ScheduleManager : IScheduleManager
    {
        private IScheduleRepository _scheduleRepository;
        private IRoomRepository _roomRepository;

        /// <summary>
        /// schedule manager intakes schedule repository and room repository
        /// </summary>
        /// <param name="scheduleRepository"></param>
        /// <param name="roomRepository"></param>
        public ScheduleManager(IScheduleRepository scheduleRepository, IRoomRepository roomRepository)
        {
            _scheduleRepository = scheduleRepository;
            _roomRepository = roomRepository;
        }
       /// <summary>
       /// add schedule
       /// </summary>
       /// <param name="body"></param>
       /// <returns></returns>
        public Schedule AddSchedule(ScheduleFormBody body)
        {
            int RoomID = _roomRepository.FindRoomIdByName(body.RoomName);
            return null;
        }
    }
}
