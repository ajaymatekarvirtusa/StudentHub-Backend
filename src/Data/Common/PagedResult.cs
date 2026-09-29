namespace Data.Common
{
    /// <summary>One page of results plus the information a UI needs to show pager controls.</summary>
    public class PagedResult<T>
    {
        public IReadOnlyList<T> Items { get; init; } = [];
        public int PageNumber { get; init; }
        public int PageSize { get; init; }
        public int TotalCount { get; init; }

        public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;

        public PagedResult<TOut> Map<TOut>(Func<T, TOut> map) => new()
        {
            Items = Items.Select(map).ToList(),
            PageNumber = PageNumber,
            PageSize = PageSize,
            TotalCount = TotalCount
        };
    }
}
