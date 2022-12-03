using UCS_Backend.Models;
using UCS_Backend.Models.SubModels;

namespace UCS_Backend.Interfaces
{ 
    /// <summary>
    /// creates class for instructor repo for tasks passed
    /// </summary>
    public interface IInstructorRepository : IBaseRepository<Instructor>
    {
        Task<List<Instructor>> GetAllInstructors();
        Task<Instructor?> GetInstructorById(int id);
        Task<Instructor> AddInstructor(Instructor instructor);
        Task<(bool, Instructor)> UpdateInstrutor(Instructor instructor);
        Task<bool> DeleteInstructor(Instructor instructor);
        List<ScheduleInfo> GetScheduleByInstructor(int employeeNumber);
    }
}
