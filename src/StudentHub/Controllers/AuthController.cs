using API.Models;
using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;

namespace API.Controllers
{
    [AllowAnonymous]
    public class AuthController(ITokenService _tokenService, ILogger<AuthController> _logger) : ApiControllerBase
    {
        /// <summary>Returns a JWT bearer token. Every user is issued the Admin role.</summary>
        [HttpPost("token")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public IActionResult Token([FromBody] LoginRequest? request)
        {
            if (request is null)
            {
                _logger.LogWarning("POST api/Auth/token - request body is null");
                return BadRequest(new ErrorResponse("Username and password are required."));
            }

            // Never log the password.
            _logger.LogInformation("POST api/Auth/token called. Username={Username}", request.Username);

            var token = _tokenService.GenerateToken(request.Username);
            return Ok(token);
        }
    }
}
