using UCS_Backend.Models;

namespace UCS_Backend.Interfaces.IRepositories
{
    public interface IRoomRepository: IBaseRepository<Room>
    {
        int FindRoomIdByName(string name);
    }
}
