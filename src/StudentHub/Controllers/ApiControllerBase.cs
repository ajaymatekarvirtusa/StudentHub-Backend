using API.Models;
using Data.Common;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    /// <summary>
    /// Settings shared by every API controller (route, JSON, and the error responses any endpoint can return).
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public abstract class ApiControllerBase : ControllerBase
    {
        /// <summary>Turns a failed ServiceResult into 404 / 409 with the standard ErrorResponse body.</summary>
        protected IActionResult ToErrorResult<T>(ServiceResult<T> result) => result.Status switch
        {
            ServiceResultStatus.NotFound => NotFound(new ErrorResponse(result.Error ?? "Not found.")),
            ServiceResultStatus.Conflict => Conflict(new ErrorResponse(result.Error ?? "Conflict.")),
            _ => throw new InvalidOperationException("ToErrorResult called with a successful result.")
        };
    }
}
