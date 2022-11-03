using UCS_Backend.Models;
using UCS_Backend.Models.SubModels;

namespace UCS_Backend.Interfaces.IRepositories
{
    public interface IRoomRepository: IBaseRepository<Room>
    {
        int FindRoomIdByName(string name);

        List<ScheduleInfo> GetScheduleByRoomNumber(int roomNumber);
    }
}
