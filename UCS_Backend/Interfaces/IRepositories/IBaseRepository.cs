namespace UCS_Backend.Interfaces
{
/// <summary>
/// creates class for base repo to pass Updates / Delete /  FindById
/// </summary>
/// <typeparam name="T"></typeparam>
    public interface IBaseRepository<T> where T : class
    {
        IEnumerable<T> GetAll { get;}

        T Add(T entity);
        void Update(T entity);
        void Delete(T entity);  
        T? FindById(int id);
    }
}
