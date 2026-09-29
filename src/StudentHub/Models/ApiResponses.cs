namespace API.Models
{
    /// <summary>Error body returned by the API, e.g. { "error": "Student not found." }</summary>
    public record ErrorResponse(string Error);

    /// <summary>Returned after a successful update, e.g. { "id": 5 }</summary>
    public record IdResponse(int Id);

    /// <summary>Returned after a successful delete, e.g. { "success": true }</summary>
    public record SuccessResponse(bool Success);
}
