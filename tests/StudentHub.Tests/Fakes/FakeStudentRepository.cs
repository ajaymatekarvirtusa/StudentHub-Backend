using Data.Models;
using Repositories.Interface;

namespace StudentHub.Tests.Fakes
{
    /// <summary>Hand-written test double for IStudentRepository (no database needed).</summary>
    public class FakeStudentRepository : IStudentRepository
    {
        public StudentDetail? GetByIdResult { get; set; }
        public IEnumerable<StudentDetail> GetAllResult { get; set; } = [];
        public int AddResult { get; set; }
        public int UpdateResult { get; set; }
        public bool DeleteResult { get; set; }

        public StudentDetail? LastAdded { get; private set; }
        public StudentDetail? LastUpdated { get; private set; }
        public (int StudentId, int ModifiedBy)? LastDeleted { get; private set; }

        public Task<int> AddStudentAsync(StudentDetail studentDetail)
        {
            LastAdded = studentDetail;
            return Task.FromResult(AddResult);
        }

        public Task<StudentDetail?> GetStudentByIdAsync(int studentId) => Task.FromResult(GetByIdResult);

        public Task<IEnumerable<StudentDetail>> GetAllStudentsAsync() => Task.FromResult(GetAllResult);

        public Task<int> UpdateStudentAsync(StudentDetail studentDetail)
        {
            LastUpdated = studentDetail;
            return Task.FromResult(UpdateResult);
        }

        public Task<bool> DeleteStudentAsync(int studentId, int modifiedBy)
        {
            LastDeleted = (studentId, modifiedBy);
            return Task.FromResult(DeleteResult);
        }
    }
}
