namespace Runord.Shared.Base
{
    public class Result<T>
    {
        public bool IsSuccess { get; init; }
        public T? Data { get; init; }
        public string? ErrorMessage { get; init; }

        public static Result<T> Success(T data) =>
            new() { IsSuccess = true, Data = data };

        public static Result<T> Failure(string errorMessage) =>
            new() { IsSuccess = false, ErrorMessage = errorMessage };
    }

    public class PagedResponse<T>
    {
        public IEnumerable<T> Items { get; init; } = Enumerable.Empty<T>();
        public int TotalCount { get; init; }
        public int PageNumber { get; init; }
        public int PageSize { get; init; }

        // Вычисляемое свойство, удобно для биндинга кнопок в WPF
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    }
}
