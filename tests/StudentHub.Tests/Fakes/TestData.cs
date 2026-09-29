using Data.Models;

namespace StudentHub.Tests.Fakes
{
    public static class TestData
    {
        public static StudentDetail Student(int id = 1, string name = "Ravi Kumar") => new()
        {
            Id = id,
            Name = name,
            Dob = "2002-05-14",
            MobileNo = "9876543210",
            CreatedDate = new DateTime(2026, 1, 1),
            CreatedBy = 1
        };
    }
}
