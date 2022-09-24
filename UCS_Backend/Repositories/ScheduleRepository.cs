using UCS_Backend.Data;
using UCS_Backend.Interfaces;
using UCS_Backend.Models;

namespace UCS_Backend.Repositories
{
    public class ScheduleRepository : IScheduleRepository
    {
        private DataContext _dataContext;
        public ScheduleRepository(DataContext dataContext)
        {
            this._dataContext = dataContext;
        }
        public List<Schedule> GetAllSchedules()
        { 
            var res = this._dataContext.Schedules.ToList();

            return res;
        }
    }
}
