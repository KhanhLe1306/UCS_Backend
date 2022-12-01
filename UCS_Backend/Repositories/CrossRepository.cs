using UCS_Backend.Data;
using UCS_Backend.Interfaces.IRepositories;
using UCS_Backend.Models;

namespace UCS_Backend.Repositories
{

    /// <summary>
    /// Creates a class for CrossRepositoty
    /// </summary> 
    public class CrossRepository : ICrossRepository
    {
        private DataContext dataContext;

        public CrossRepository(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }
        public IEnumerable<Cross> GetAll => throw new NotImplementedException();

        public Cross Add(Cross entity)
        {
            var temp = dataContext.Cross.Where(x => x.ClssID1 == entity.ClssID1 && x.ClssID2 == entity.ClssID2).FirstOrDefault();
            if (temp == null) // add
            {
                var res = dataContext.Cross.Add(entity).Entity;
                dataContext.SaveChanges();
                return res;
            }
            return entity;
        }

        public void Delete(Cross entity)
        {
            throw new NotImplementedException();
        }

        public Cross? FindById(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(Cross entity)
        {
            throw new NotImplementedException();
        }
    }
}
