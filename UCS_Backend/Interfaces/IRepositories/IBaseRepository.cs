namespace UCS_Backend.Interfaces
{
    public interface IBaseRepository<T> where T : class
    {
        IEnumerable<T> GetAll { get;}

        T Add(T entity);
        void Update(T entity);
        void Delete(T entity);  
        T? FindById(int id);
    }
}
