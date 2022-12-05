using UCS_Backend.Models;
using UCS_Backend.Models.SubModels;
using UCS_Backend.Repositories;

namespace UCS_Backend.Interfaces
{
/// <summary>
/// Creates class for scheduleRepo
/// </summary>
    public interface IScheduleRepository : IBaseRepository<Schedule>
    {
        List<Schedule> GetAllSchedules();
        SuccessInfo ValidateInsert(AddClassModel addClassModel);
        void AddClass(AddClassModel addClassModel);
    }
}
