using Data.Entities;

namespace Data.Models.Masters
{
    /// <summary>What the API returns for a country (IsDeleted is never exposed).</summary>
    public class CountryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }

        public static CountryResponse From(Country country) => new()
        {
            Id = country.Id,
            Name = country.Name,
            Code = country.Code,
            IsActive = country.IsActive,
            CreatedDate = country.CreatedDate,
            CreatedBy = country.CreatedBy,
            ModifiedDate = country.ModifiedDate,
            ModifiedBy = country.ModifiedBy
        };
    }
}
