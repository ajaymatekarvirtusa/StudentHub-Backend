namespace Data.Entities
{
    /// <summary>
    /// Common columns for every master table (Country, State, City, ...).
    /// Audit fields are filled automatically by ApplicationDbContext.SaveChangesAsync.
    /// </summary>
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? ModifiedDate { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
