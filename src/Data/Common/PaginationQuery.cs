using System.ComponentModel;

namespace Data.Common
{
    /// <summary>Standard paging query string: ?pageNumber=1&amp;pageSize=5. Reusable for every list endpoint.</summary>
    public class PaginationQuery
    {
        public const int DefaultPageSize = 5;
        public const int MaxPageSize = 100;

        [DefaultValue(1)]
        public int PageNumber { get; set; } = 1;
        [DefaultValue(DefaultPageSize)]
        public int PageSize { get; set; } = DefaultPageSize;
    }
}
