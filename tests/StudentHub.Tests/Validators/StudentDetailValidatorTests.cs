using API.Validators;
using Data.Models;
using StudentHub.Tests.Fakes;

namespace StudentHub.Tests.Validators
{
    public class StudentDetailValidatorTests
    {
        private readonly StudentDetailValidator _validator = new();

        private static string Date(int daysFromToday) => DateTime.Today.AddDays(daysFromToday).ToString("yyyy-MM-dd");

        private List<string> ErrorsFor(StudentDetail student, string property) =>
            _validator.Validate(student).Errors.Where(e => e.PropertyName == property).Select(e => e.ErrorMessage).ToList();

        [Fact]
        public void ValidStudent_HasNoErrors()
        {
            Assert.True(_validator.Validate(TestData.Student()).IsValid);
        }

        [Fact]
        public void OnlyName_IsValid_BecauseDobAndMobileAreOptional()
        {
            var student = new StudentDetail { Name = "Asha", Dob = null, MobileNo = null };

            Assert.True(_validator.Validate(student).IsValid);
        }

        // ---------- Name ----------

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Name_WhenEmpty_IsRequired(string name)
        {
            var student = TestData.Student(name: name);

            Assert.Contains("Name is required.", ErrorsFor(student, nameof(StudentDetail.Name)));
        }

        [Fact]
        public void Name_WhenOver150Characters_IsInvalid()
        {
            var student = TestData.Student(name: new string('a', 151));

            Assert.Contains("Name must not exceed 150 characters.", ErrorsFor(student, nameof(StudentDetail.Name)));
        }

        [Fact]
        public void Name_WhenExactly150Characters_IsValid()
        {
            var student = TestData.Student(name: new string('a', 150));

            Assert.Empty(ErrorsFor(student, nameof(StudentDetail.Name)));
        }

        // ---------- MobileNo ----------

        [Theory]
        [InlineData("12345")]
        [InlineData("12345678901")]
        [InlineData("98765abcde")]
        [InlineData("98765 43210")]
        public void MobileNo_WhenNotExactly10Digits_IsInvalid(string mobileNo)
        {
            var student = TestData.Student();
            student.MobileNo = mobileNo;

            Assert.Contains("Mobile number must be exactly 10 digits.", ErrorsFor(student, nameof(StudentDetail.MobileNo)));
        }

        [Theory]
        [InlineData("9876543210")]
        [InlineData("")]
        public void MobileNo_WhenTenDigitsOrEmpty_IsValid(string mobileNo)
        {
            var student = TestData.Student();
            student.MobileNo = mobileNo;

            Assert.Empty(ErrorsFor(student, nameof(StudentDetail.MobileNo)));
        }

        [Fact]
        public void MobileNo_WhenNull_IsValid()
        {
            var student = TestData.Student();
            student.MobileNo = null;

            Assert.Empty(ErrorsFor(student, nameof(StudentDetail.MobileNo)));
        }

        // ---------- Dob ----------

        [Fact]
        public void Dob_WhenInThePast_IsValid()
        {
            var student = TestData.Student();
            student.Dob = Date(-1);

            Assert.Empty(ErrorsFor(student, nameof(StudentDetail.Dob)));
        }

        [Theory]
        [InlineData(0)]   // today
        [InlineData(1)]   // tomorrow
        public void Dob_WhenTodayOrFuture_IsInvalid(int daysFromToday)
        {
            var student = TestData.Student();
            student.Dob = Date(daysFromToday);

            Assert.NotEmpty(ErrorsFor(student, nameof(StudentDetail.Dob)));
        }

        [Theory]
        [InlineData("14-05-2002")]
        [InlineData("2002/05/14")]
        [InlineData("not-a-date")]
        [InlineData("2002-02-30")]
        public void Dob_WhenNotValidYyyyMmDd_IsInvalid(string dob)
        {
            var student = TestData.Student();
            student.Dob = dob;

            Assert.Contains("Date of birth must be a valid past date in yyyy-MM-dd format.",
                ErrorsFor(student, nameof(StudentDetail.Dob)));
        }
    }
}
