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
    /// <summary>
    /// creates cross repo
    /// </summary>
    /// <param name="dataContext"></param>
        public CrossRepository(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }
        public IEnumerable<Cross> GetAll => throw new NotImplementedException();
    /// <summary>
    /// add cross entity for cross listing
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
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
    /// <summary>
    /// delete cross entity
    /// </summary>
    /// <param name="entity"></param>
        public void Delete(Cross entity)
        {
            throw new NotImplementedException();
        }
    /// <summary>
    /// find cross entity by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
        public Cross? FindById(int id)
        {
            throw new NotImplementedException();
        }
/// <summary>
/// update cross entity
/// </summary>
/// <param name="entity"></param>
        public void Update(Cross entity)
        {
            throw new NotImplementedException();
        }
    }
}
