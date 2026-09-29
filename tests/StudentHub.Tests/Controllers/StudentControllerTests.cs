using API.Controllers;
using API.Models;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using StudentHub.Tests.Fakes;

namespace StudentHub.Tests.Controllers
{
    public class StudentControllerTests
    {
        private readonly FakeStudentService _service = new();
        private readonly StudentController _controller;

        public StudentControllerTests()
        {
            _controller = new StudentController(_service, NullLogger<StudentController>.Instance);
        }

        // ---------- GET api/Student/{id} ----------

        [Fact]
        public async Task GetStudent_WhenFound_ReturnsOkWithStudent()
        {
            var student = TestData.Student(id: 5);
            _service.GetByIdResult = student;

            var result = await _controller.GetStudent(5);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Same(student, ok.Value);
        }

        [Fact]
        public async Task GetStudent_WhenNotFound_ReturnsNotFoundWithError()
        {
            _service.GetByIdResult = null;

            var result = await _controller.GetStudent(99);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            var error = Assert.IsType<ErrorResponse>(notFound.Value);
            Assert.Equal("Student not found.", error.Error);
        }

        // ---------- GET api/Student ----------

        [Fact]
        public async Task GetAll_ReturnsOkWithAllStudents()
        {
            _service.GetAllResult = [TestData.Student(1), TestData.Student(2, "Asha")];

            var result = await _controller.GetAll();

            var ok = Assert.IsType<OkObjectResult>(result);
            var students = Assert.IsAssignableFrom<IEnumerable<StudentDetail>>(ok.Value);
            Assert.Equal(2, students.Count());
        }

        [Fact]
        public async Task GetAll_WhenNoStudents_ReturnsOkWithEmptyList()
        {
            _service.GetAllResult = [];

            var result = await _controller.GetAll();

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Empty(Assert.IsAssignableFrom<IEnumerable<StudentDetail>>(ok.Value));
        }

        // ---------- POST api/Student ----------

        [Fact]
        public async Task Create_WhenBodyIsNull_ReturnsBadRequest_AndDoesNotCallService()
        {
            var result = await _controller.Create(null);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var error = Assert.IsType<ErrorResponse>(badRequest.Value);
            Assert.Equal("Student details are required.", error.Error);
            Assert.Equal(0, _service.CallCount);
        }

        [Fact]
        public async Task Create_WhenValid_ReturnsCreatedAtGetStudent_WithNewId()
        {
            var student = TestData.Student(id: 0);
            _service.AddResult = 42;

            var result = await _controller.Create(student);

            var created = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(StudentController.GetStudent), created.ActionName);
            Assert.Equal(42, created.RouteValues!["id"]);
            Assert.Same(student, created.Value);
            Assert.Same(student, _service.LastAdded);
        }

        // ---------- PUT api/Student/{id} ----------

        [Fact]
        public async Task Update_WhenBodyIsNull_ReturnsBadRequest_AndDoesNotCallService()
        {
            var result = await _controller.Update(1, null);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Student details are required.", Assert.IsType<ErrorResponse>(badRequest.Value).Error);
            Assert.Equal(0, _service.CallCount);
        }

        [Fact]
        public async Task Update_WhenRouteIdDoesNotMatchBodyId_ReturnsBadRequest_AndDoesNotCallService()
        {
            var result = await _controller.Update(1, TestData.Student(id: 2));

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Id in the URL does not match Id in the request body.",
                Assert.IsType<ErrorResponse>(badRequest.Value).Error);
            Assert.Equal(0, _service.CallCount);
        }

        [Fact]
        public async Task Update_WhenStudentNotFound_ReturnsNotFound()
        {
            _service.UpdateResult = 0;

            var result = await _controller.Update(7, TestData.Student(id: 7));

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Student not found or not updated.", Assert.IsType<ErrorResponse>(notFound.Value).Error);
        }

        [Fact]
        public async Task Update_WhenSuccessful_ReturnsOkWithId()
        {
            var student = TestData.Student(id: 7);
            _service.UpdateResult = 7;

            var result = await _controller.Update(7, student);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(new IdResponse(7), ok.Value);
            Assert.Same(student, _service.LastUpdated);
        }

        // ---------- DELETE api/Student/{id} ----------

        [Fact]
        public async Task Delete_WhenStudentNotFound_ReturnsNotFound()
        {
            _service.DeleteResult = false;

            var result = await _controller.Delete(3, modifiedBy: 1);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Student not found or not deleted.", Assert.IsType<ErrorResponse>(notFound.Value).Error);
        }

        [Fact]
        public async Task Delete_WhenSuccessful_ReturnsOk_AndPassesIdAndModifiedBy()
        {
            _service.DeleteResult = true;

            var result = await _controller.Delete(3, modifiedBy: 9);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(new SuccessResponse(true), ok.Value);
            Assert.Equal((3, 9), _service.LastDeleted);
        }
    }
}
