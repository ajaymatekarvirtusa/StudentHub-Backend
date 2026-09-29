using Microsoft.Extensions.Logging.Abstractions;
using Services.Service;
using StudentHub.Tests.Fakes;

namespace StudentHub.Tests.Services
{
    public class StudentServiceTests
    {
        private readonly FakeStudentRepository _repository = new();
        private readonly StudentService _service;

        public StudentServiceTests()
        {
            _service = new StudentService(_repository, NullLogger<StudentService>.Instance);
        }

        [Fact]
        public async Task AddStudentAsync_PassesStudentToRepository_AndReturnsNewId()
        {
            var student = TestData.Student(id: 0);
            _repository.AddResult = 10;

            var id = await _service.AddStudentAsync(student);

            Assert.Equal(10, id);
            Assert.Same(student, _repository.LastAdded);
        }

        [Fact]
        public async Task GetStudentByIdAsync_ReturnsRepositoryResult()
        {
            var student = TestData.Student(id: 4);
            _repository.GetByIdResult = student;

            Assert.Same(student, await _service.GetStudentByIdAsync(4));
        }

        [Fact]
        public async Task GetStudentByIdAsync_WhenNotFound_ReturnsNull()
        {
            _repository.GetByIdResult = null;

            Assert.Null(await _service.GetStudentByIdAsync(4));
        }

        [Fact]
        public async Task GetAllStudentsAsync_ReturnsAllStudents()
        {
            _repository.GetAllResult = [TestData.Student(1), TestData.Student(2), TestData.Student(3)];

            var students = await _service.GetAllStudentsAsync();

            Assert.Equal(3, students.Count());
        }

        [Theory]
        [InlineData(5, 5)]
        [InlineData(0, 0)]
        public async Task UpdateStudentAsync_ReturnsRepositoryResult(int repositoryResult, int expected)
        {
            _repository.UpdateResult = repositoryResult;

            Assert.Equal(expected, await _service.UpdateStudentAsync(TestData.Student(id: 5)));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task DeleteStudentAsync_ReturnsRepositoryResult_AndPassesArguments(bool deleted)
        {
            _repository.DeleteResult = deleted;

            var result = await _service.DeleteStudentAsync(8, modifiedBy: 2);

            Assert.Equal(deleted, result);
            Assert.Equal((8, 2), _repository.LastDeleted);
        }
    }
}
