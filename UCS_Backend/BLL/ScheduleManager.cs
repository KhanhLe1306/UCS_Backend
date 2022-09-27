using UCS_Backend.Interfaces;
using UCS_Backend.Interfaces.IManagers;
using UCS_Backend.Interfaces.IRepositories;
using UCS_Backend.Models;

namespace UCS_Backend.BLL
{
    public class ScheduleManager : IScheduleManager
    {
        private IScheduleRepository _scheduleRepository;
        private IRoomRepository _roomRepository;
        public ScheduleManager(IScheduleRepository scheduleRepository, IRoomRepository roomRepository)
        {
            _scheduleRepository = scheduleRepository;
            _roomRepository = roomRepository;
        }
        public Schedule AddSchedule(ScheduleFormBody body)
        {
            int RoomID = _roomRepository.FindRoomIdByName(body.RoomName);
            return null;
        }
    }
}
