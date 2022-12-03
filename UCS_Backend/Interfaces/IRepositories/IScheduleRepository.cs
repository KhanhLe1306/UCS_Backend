using UCS_Backend.Models;
using UCS_Backend.Models.SubModels;
using UCS_Backend.Repositories;

namespace UCS_Backend.Interfaces
{
    public interface IScheduleRepository : IBaseRepository<Schedule>
    {
        List<Schedule> GetAllSchedules();
        SuccessInfo ValidateInsert(AddClassModel addClassModel);
    }
}
