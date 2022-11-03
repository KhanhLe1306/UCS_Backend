using UCS_Backend.Models;

namespace UCS_Backend.Interfaces.IRepositories
{
    public interface IClassRepository
    {
        int AddNewClass(ClassModel classModel);
        int FindClssID(string catalogNumber, string section);
    }
}
