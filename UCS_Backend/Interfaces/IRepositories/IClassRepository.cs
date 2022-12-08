using UCS_Backend.Models;
using UCS_Backend.Models.SubModels;

namespace UCS_Backend.Interfaces.IRepositories
{
    /// <summary>
    /// creates class for class repository to ad new class or find classID
    /// </summary>
    public interface IClassRepository
    {
        int AddNewClass(ClassModel classModel);
        int FindClssID(string catalogNumber, string section);
        void RemoveClass(string classID);
        int GetClassIdByCourseAndSection(string courseNumber, string sectionNumber, string enrollment, string subjectCode, string coursetitle, int clssId);
        int GetScheduleIdByClssId(int clssId);
        SuccessInfo ValidateClassUpdate(UpdateClassModel updateClassModel);
    }
}
