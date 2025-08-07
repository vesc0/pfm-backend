namespace PFM.Application.Common.Pagination
{
    public class PagedResult<T>
    {
        public int PageSize { get; init; }
        public int Page { get; init; }
        public int TotalCount { get; init; }
        public string? SortBy { get; init; }
        public string? SortOrder { get; init; }
        public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();
    }
}