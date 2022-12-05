using UCS_Backend.Models;

namespace UCS_Backend.Interfaces.IRepositories
{

    /// <summary>
    /// Creates class for interface TimeRepo
    /// </summary>
    public interface ITimeRepository : IBaseRepository<Time>
    {
        int GetTimeId(string startTime, string endTime);
    }
}
