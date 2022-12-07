using UCS_Backend.Models;
using UCS_Backend.Models.SubModels;
using UCS_Backend.Repositories;

namespace UCS_Backend.Interfaces.IRepositories
{
    /// <summary>
    /// creates class for room repo to find room id by name
    /// </summary>
    public interface IRoomRepository: IBaseRepository<Room>
    {
        int FindRoomIdByName(string name);

        List<ScheduleInfo> GetScheduleByRoomNumber(int roomNumber);

        int GetRoomIdByRoomName(string buildingCode, string roomNumber);
    }
}
