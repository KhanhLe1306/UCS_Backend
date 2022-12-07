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

        /// <summary>
        /// creates time repo
        /// </summary>
        /// <param name="context"></param>
        public TimeRepository(DataContext context)
        {
            this._dataContext = context;
        }

        public IEnumerable<Time> GetAll => throw new NotImplementedException();

        /// <summary>
        /// creates a method to add time and returns time as a data context 
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
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

        /// <summary>
        /// deletes the incorrect time entity
        /// </summary>
        /// <param name="entity"></param>
        public void Delete(Time entity)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// find id by time
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Time? FindById(int id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public int GetTimeId(string startTime, string endTime)
        {
            var time = this._dataContext.Time.Where(x => x.StartTime == Int32.Parse(startTime) && x.EndTime == Int32.Parse(endTime)).FirstOrDefault();
            if (time != null) { 
                return time.TimeId; 
            }else {
                var timeId = this._dataContext.Time.Add(new Time
                {
                    StartTime = Int32.Parse(startTime),
                    EndTime = Int32.Parse(endTime)
                }).Entity.TimeId;
                this._dataContext.SaveChanges();
                return timeId;
            }
        }

        /// <summary>
        /// updates time entity
        /// </summary>
        /// <param name="entity"></param>
        public void Update(Time entity)
        {
            throw new NotImplementedException();
        }
    }
}
