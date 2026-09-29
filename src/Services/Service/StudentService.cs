using Data.Models;
using Microsoft.Extensions.Logging;
using Repositories.Interface;
using Services.Interface;

namespace Services.Service
{
    public class StudentService(IStudentRepository _studentRepository, ILogger<StudentService> _logger) : IStudentService
    {
        public async Task<int> AddStudentAsync(StudentDetail studentDetail)
        {
            var studentId = await _studentRepository.AddStudentAsync(studentDetail);

            if (studentId > 0)
                _logger.LogInformation("Student created. StudentId={StudentId} CreatedBy={CreatedBy}", studentId, studentDetail.CreatedBy);
            else
                _logger.LogWarning("Student insert returned no id. CreatedBy={CreatedBy}", studentDetail.CreatedBy);

            return studentId;
        }

        public async Task<bool> DeleteStudentAsync(int studentId, int modifiedBy)
        {
            var deleted = await _studentRepository.DeleteStudentAsync(studentId, modifiedBy);

            if (deleted)
                _logger.LogInformation("Student deleted. StudentId={StudentId} ModifiedBy={ModifiedBy}", studentId, modifiedBy);
            else
                _logger.LogWarning("Student delete failed - not found or already deleted. StudentId={StudentId}", studentId);

            return deleted;
        }

        public async Task<IEnumerable<StudentDetail>> GetAllStudentsAsync()
        {
            var students = (await _studentRepository.GetAllStudentsAsync()).ToList();
            _logger.LogDebug("Fetched {Count} students", students.Count);
            return students;
        }

        public async Task<StudentDetail?> GetStudentByIdAsync(int studentId)
        {
            var student = await _studentRepository.GetStudentByIdAsync(studentId);

            if (student is null)
                _logger.LogDebug("Student not found. StudentId={StudentId}", studentId);

            return student;
        }

        public async Task<int> UpdateStudentAsync(StudentDetail studentDetail)
        {
            var updatedId = await _studentRepository.UpdateStudentAsync(studentDetail);

            if (updatedId > 0)
                _logger.LogInformation("Student updated. StudentId={StudentId} ModifiedBy={ModifiedBy}", updatedId, studentDetail.ModifiedBy);
            else
                _logger.LogWarning("Student update failed - not found or deleted. StudentId={StudentId}", studentDetail.Id);

            return updatedId;
        }
    }
}
