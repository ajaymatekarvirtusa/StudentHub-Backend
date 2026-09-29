using API.Models;
using Data.Common;
using Data.Models.Masters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interface.Masters;

namespace API.Controllers.Masters
{
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public class CountryController(ICountryService _countryService, ILogger<CountryController> _logger) : ApiControllerBase
    {
        /// <summary>
        /// Countries, 5 per page by default.
        /// Example: GET api/Country?pageNumber=2&amp;pageSize=5&amp;search=ind&amp;activeOnly=true
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<CountryResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAll([FromQuery] CountryQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInformation("GET api/Country called. Page={PageNumber} Size={PageSize} Search={Search} ActiveOnly={ActiveOnly}",
                query.PageNumber, query.PageSize, query.Search, query.ActiveOnly);
            return Ok(await _countryService.GetPagedAsync(query, cancellationToken));
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(CountryResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("GET api/Country/{CountryId} called", id);

            var country = await _countryService.GetByIdAsync(id, cancellationToken);
            return country is null ? NotFound(new ErrorResponse("Country not found.")) : Ok(country);
        }

        [HttpPost]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(CountryResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create([FromBody] CountryRequest? request, CancellationToken cancellationToken)
        {
            if (request is null)
                return BadRequest(new ErrorResponse("Country details are required."));

            _logger.LogInformation("POST api/Country called");

            var result = await _countryService.CreateAsync(request, cancellationToken);
            return result.IsSuccess
                ? CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value)
                : ToErrorResult(result);
        }

        [HttpPut("{id:int}")]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(CountryResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Update(int id, [FromBody] CountryRequest? request, CancellationToken cancellationToken)
        {
            if (request is null)
                return BadRequest(new ErrorResponse("Country details are required."));

            _logger.LogInformation("PUT api/Country/{CountryId} called", id);

            var result = await _countryService.UpdateAsync(id, request, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : ToErrorResult(result);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(SuccessResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            _logger.LogInformation("DELETE api/Country/{CountryId} called", id);

            return await _countryService.DeleteAsync(id, cancellationToken)
                ? Ok(new SuccessResponse(true))
                : NotFound(new ErrorResponse("Country not found."));
        }
    }
}
