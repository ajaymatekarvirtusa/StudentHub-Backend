namespace Data.Models
{
    public class StudentDetail
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Dob { get; set; }
        public string? MobileNo { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public int IsDeleted { get; set; }
    }
}
