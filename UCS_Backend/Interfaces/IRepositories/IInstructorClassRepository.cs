using UCS_Backend.Models;

namespace UCS_Backend.Interfaces.IRepositories
{
    public interface IInstructorClassRepository : IBaseRepository<InstructorClass>
    {
        Task<List<InstructorClass>> GetInstructorClasses();
        Task<InstructorClass?> GetInstructorClassById(int id);
        Task<InstructorClass> AddInstructorClass(InstructorClass instructorClass);
        Task<(bool, InstructorClass)> UpdateInstructorClass(InstructorClass instructorClass);
        Task<bool> DeleteInstructorClass(InstructorClass instructorClass);
    }
}
