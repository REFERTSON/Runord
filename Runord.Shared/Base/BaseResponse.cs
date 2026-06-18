namespace Runord.Shared.Base
{
    public readonly record struct Response<T>
    {
        public bool IsSuccess { get; init; }
        public T? Data { get; init; }
        public string? ErrorMessage { get; init; }

        public static Response<T> Success(T data) =>
            new() { IsSuccess = true, Data = data };

        public static Response<T> Failure(string errorMessage) =>
            new() { IsSuccess = false, ErrorMessage = errorMessage };
    }
}
