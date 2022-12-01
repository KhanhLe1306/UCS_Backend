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
        /// Updates the ScheduleRepository and RoomRepository to be passed
        /// </summary>
        /// <param name="roomRepository">Room to be passed in</param>
        public ScheduleManager(IScheduleRepository scheduleRepository, IRoomRepository roomRepository)
        {
            _scheduleRepository = scheduleRepository;
            _roomRepository = roomRepository;
        }
        public Schedule AddSchedule(ScheduleFormBody body)

         /// <summary>
         /// Adds schedule to the schedule form body
         /// </summary>
         /// <param name="ScheduleFormBody">Schedule to be passed to ID</param>
        {
            int RoomID = _roomRepository.FindRoomIdByName(body.RoomName);
            return null;
        }
    }
}
