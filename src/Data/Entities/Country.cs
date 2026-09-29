namespace Data.Entities
{
    public class Country : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        /// <summary>ISO 3166 code, e.g. "IN" or "IND".</summary>
        public string Code { get; set; } = string.Empty;
    }
}
