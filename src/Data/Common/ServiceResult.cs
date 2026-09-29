namespace Data.Common
{
    public enum ServiceResultStatus
    {
        Success,
        NotFound,
        Conflict
    }

    /// <summary>
    /// Outcome of a service call, so controllers can return 200 / 404 / 409 without exceptions.
    /// Reusable for every master (Country, State, ...).
    /// </summary>
    public class ServiceResult<T>
    {
        public ServiceResultStatus Status { get; private init; }
        public T? Value { get; private init; }
        public string? Error { get; private init; }

        public bool IsSuccess => Status == ServiceResultStatus.Success;

        public static ServiceResult<T> Success(T value) => new() { Status = ServiceResultStatus.Success, Value = value };
        public static ServiceResult<T> NotFound(string error) => new() { Status = ServiceResultStatus.NotFound, Error = error };
        public static ServiceResult<T> Conflict(string error) => new() { Status = ServiceResultStatus.Conflict, Error = error };
    }
}
