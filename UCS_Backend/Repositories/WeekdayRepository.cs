using UCS_Backend.Data;
using UCS_Backend.Interfaces.IRepositories;
using UCS_Backend.Models;

namespace UCS_Backend.Repositories
{
    /// <summary>
    /// Creates a class for WeekdayRepositoty
    /// </summary> 
    public class WeekdayRepository : IWeekdayRepository
    {
        private DataContext _dataContext;
        /// <summary>
        /// creats data context for repo
        /// </summary>
        /// <param name="context"></param>
        public WeekdayRepository(DataContext context)
        {
            this._dataContext = context;
        }
        public IEnumerable<Weekday> GetAll => throw new NotImplementedException();
        /// <summary>
        /// creates a weekday id for the entity to be created inside of data context
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Weekday Add(Weekday entity)
        {
            int weekdayId = (from t in _dataContext.Weekdays where t.Description == entity.Description select t.WeekdayId).FirstOrDefault();
            if (weekdayId == 0)
            {
                var res = _dataContext.Weekdays.Add(entity).Entity;
                _dataContext.SaveChanges();
                return res;
            }
            else
            {
                return new Weekday
                {
                    WeekdayId = weekdayId,
                    Description = entity.Description
                };
            }
        }
        /// <summary>
        /// deletes weekday
        /// </summary>
        /// <param name="entity"></param>
        public void Delete(Weekday entity)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// finds the weekday by int
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Weekday? FindById(int id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// updates the week day
        /// </summary>
        /// <param name="entity"></param>
        public void Update(Weekday entity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        public int GetWeekDaysIdByDescription(string description)
        {
            var weekdays = _dataContext.Weekdays.Where(x => x.Description == description).FirstOrDefault();
            if(weekdays == null) // Add
            {
                var weekdayId = this._dataContext.Add(new Weekday
                {
                    Description = description,
                }).Entity.WeekdayId;
                this._dataContext.SaveChanges();
                return weekdayId;
            }
            else
            {
                return weekdays.WeekdayId;
            }
        }
    }
}
