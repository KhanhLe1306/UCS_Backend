using UCS_Backend.Data;
using UCS_Backend.Interfaces.IRepositories;
using UCS_Backend.Models;

namespace UCS_Backend.Repositories
{
    public class WeekdayRepository : IWeekdayRepository
    {
        private DataContext _dataContext;

        public WeekdayRepository(DataContext context)
        {
            this._dataContext = context;
        }
        public IEnumerable<Weekday> GetAll => throw new NotImplementedException();

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

        public void Delete(Weekday entity)
        {
            throw new NotImplementedException();
        }

        public Weekday? FindById(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(Weekday entity)
        {
            throw new NotImplementedException();
        }
    }
}
