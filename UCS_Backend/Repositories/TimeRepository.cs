using UCS_Backend.Data;
using UCS_Backend.Interfaces.IRepositories;
using UCS_Backend.Models;

namespace UCS_Backend.Repositories
{


    /// <summary>
    /// Creates a class for TimeRepositoty
    /// </summary> 
    public class TimeRepository : ITimeRepository
    {
        private DataContext _dataContext;
        public TimeRepository(DataContext context)
        {
            this._dataContext = context;
        }
        public IEnumerable<Time> GetAll => throw new NotImplementedException();

        public Time Add(Time time)
        {
            int timeId = (from t in _dataContext.Time where t.StartTime == time.StartTime && t.EndTime == time.EndTime select t.TimeId).FirstOrDefault();
            if (timeId == 0)
            {
                var res = _dataContext.Time.Add(time).Entity;
                _dataContext.SaveChanges();
                return res;
            }
            else
            {
                return new Time
                {
                    TimeId = timeId,
                    StartTime = time.StartTime,
                    EndTime = time.EndTime,
                };
            }
        }

        public void Delete(Time entity)
        {
            throw new NotImplementedException();
        }

        public Time? FindById(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(Time entity)
        {
            throw new NotImplementedException();
        }
    }
}
