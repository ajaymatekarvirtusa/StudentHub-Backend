using Data.Models;

namespace Services.Interface
{
    public interface IStudentService
    {
        Task<int> AddStudentAsync(StudentDetail studentDetail);
        Task<StudentDetail?> GetStudentByIdAsync(int studentId);
        Task<IEnumerable<StudentDetail>> GetAllStudentsAsync();
        Task<int> UpdateStudentAsync(StudentDetail studentDetail);
        Task<bool> DeleteStudentAsync(int studentId, int modifiedBy);
    }
}
