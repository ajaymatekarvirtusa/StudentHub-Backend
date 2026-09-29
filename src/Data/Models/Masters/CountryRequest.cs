namespace Data.Models.Masters
{
    /// <summary>Body for POST and PUT api/Country.</summary>
    public class CountryRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}
