using UCS_Backend.Models;

namespace UCS_Backend.Interfaces.IRepositories
{
    public interface IClassRepository
    {
        int AddNewClass(ClassModel classModel);
    }
}
