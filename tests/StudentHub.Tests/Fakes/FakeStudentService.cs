using Data.Models;
using Services.Interface;

namespace StudentHub.Tests.Fakes
{
    /// <summary>
    /// Hand-written test double for IStudentService.
    /// Set the "...Result" properties to control what the controller receives,
    /// and read the "Last..." properties to check what the controller sent.
    /// </summary>
    public class FakeStudentService : IStudentService
    {
        public StudentDetail? GetByIdResult { get; set; }
        public IEnumerable<StudentDetail> GetAllResult { get; set; } = [];
        public int AddResult { get; set; }
        public int UpdateResult { get; set; }
        public bool DeleteResult { get; set; }

        public StudentDetail? LastAdded { get; private set; }
        public StudentDetail? LastUpdated { get; private set; }
        public (int StudentId, int ModifiedBy)? LastDeleted { get; private set; }
        public int CallCount { get; private set; }

        public Task<int> AddStudentAsync(StudentDetail studentDetail)
        {
            CallCount++;
            LastAdded = studentDetail;
            return Task.FromResult(AddResult);
        }

        public Task<StudentDetail?> GetStudentByIdAsync(int studentId)
        {
            CallCount++;
            return Task.FromResult(GetByIdResult);
        }

        public Task<IEnumerable<StudentDetail>> GetAllStudentsAsync()
        {
            CallCount++;
            return Task.FromResult(GetAllResult);
        }

        public Task<int> UpdateStudentAsync(StudentDetail studentDetail)
        {
            CallCount++;
            LastUpdated = studentDetail;
            return Task.FromResult(UpdateResult);
        }

        public Task<bool> DeleteStudentAsync(int studentId, int modifiedBy)
        {
            CallCount++;
            LastDeleted = (studentId, modifiedBy);
            return Task.FromResult(DeleteResult);
        }
    }
}
