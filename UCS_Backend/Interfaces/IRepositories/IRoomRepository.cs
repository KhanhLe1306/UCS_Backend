using UCS_Backend.Models;
using UCS_Backend.Models.SubModels;
using UCS_Backend.Repositories;

namespace UCS_Backend.Interfaces.IRepositories
{
    public interface IRoomRepository: IBaseRepository<Room>
    {
        int FindRoomIdByName(string name);

        List<ScheduleInfo> GetScheduleByRoomNumber(int roomNumber);
    }
}
