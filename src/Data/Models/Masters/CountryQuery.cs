using Data.Common;

namespace Data.Models.Masters
{
    /// <summary>GET api/Country?pageNumber=1&amp;pageSize=5&amp;search=ind&amp;activeOnly=true</summary>
    public class CountryQuery : PaginationQuery
    {
        /// <summary>Matches part of the name or code, e.g. "ind" finds India and Indonesia.</summary>
        public string? Search { get; set; }

        public bool ActiveOnly { get; set; }
    }
}
