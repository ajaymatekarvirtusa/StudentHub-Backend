using Data.Models;

namespace Repositories.Interface
{
    public interface IStudentRepository
    {
        Task<int> AddStudentAsync(StudentDetail studentDetail);
        Task<StudentDetail?> GetStudentByIdAsync(int studentId);
        Task<IEnumerable<StudentDetail>> GetAllStudentsAsync();
        Task<int> UpdateStudentAsync(StudentDetail studentDetail);
        Task<bool> DeleteStudentAsync(int studentId, int modifiedBy);
    }
}
