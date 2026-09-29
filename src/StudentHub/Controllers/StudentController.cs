using API.Models;
using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;

namespace API.Controllers
{
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public class StudentController(IStudentService _studentService, ILogger<StudentController> _logger) : ApiControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<StudentDetail>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("GET api/Student called");

            var students = await _studentService.GetAllStudentsAsync();
            return Ok(students);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(StudentDetail), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStudent(int id)
        {
            _logger.LogInformation("GET api/Student/{StudentId} called", id);

            var student = await _studentService.GetStudentByIdAsync(id);
            if (student == null)
            {
                _logger.LogWarning("GET api/Student/{StudentId} - student not found", id);
                return NotFound(new ErrorResponse("Student not found."));
            }
            return Ok(student);
        }

        [HttpPost]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(StudentDetail), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] StudentDetail? studentDetail)
        {
            if (studentDetail is null)
            {
                _logger.LogWarning("POST api/Student - request body is null");
                return BadRequest(new ErrorResponse("Student details are required."));
            }

            _logger.LogInformation("POST api/Student called. CreatedBy={CreatedBy}", studentDetail.CreatedBy);

            var id = await _studentService.AddStudentAsync(studentDetail);
            return CreatedAtAction(nameof(GetStudent), new { id }, studentDetail);
        }

        [HttpPut("{id}")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(IdResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] StudentDetail? studentDetail)
        {
            if (studentDetail is null)
            {
                _logger.LogWarning("PUT api/Student/{StudentId} - request body is null", id);
                return BadRequest(new ErrorResponse("Student details are required."));
            }

            _logger.LogInformation("PUT api/Student/{StudentId} called. ModifiedBy={ModifiedBy}", id, studentDetail.ModifiedBy);

            if (id != studentDetail.Id)
            {
                _logger.LogWarning("PUT api/Student/{StudentId} - route id does not match body id {BodyId}", id, studentDetail.Id);
                return BadRequest(new ErrorResponse("Id in the URL does not match Id in the request body."));
            }

            var updatedId = await _studentService.UpdateStudentAsync(studentDetail);
            if (updatedId <= 0)
                return NotFound(new ErrorResponse("Student not found or not updated."));

            return Ok(new IdResponse(updatedId));
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(SuccessResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, [FromQuery] int modifiedBy)
        {
            _logger.LogInformation("DELETE api/Student/{StudentId} called. ModifiedBy={ModifiedBy}", id, modifiedBy);

            var success = await _studentService.DeleteStudentAsync(id, modifiedBy);
            if (!success)
                return NotFound(new ErrorResponse("Student not found or not deleted."));

            return Ok(new SuccessResponse(true));
        }
    }
}
