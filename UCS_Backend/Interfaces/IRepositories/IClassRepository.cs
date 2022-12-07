using UCS_Backend.Models;

namespace UCS_Backend.Interfaces.IRepositories
{
    /// <summary>
    /// creates class for class repository to ad new class or find classID
    /// </summary>
    public interface IClassRepository
    {
        int AddNewClass(ClassModel classModel);
        int FindClssID(string catalogNumber, string section);
        int GetClassIdByCourseAndSection(string courseNumber, string sectionNumber, string enrollment, string subjectCode, string coursetitle, int clssId);
    }
}
