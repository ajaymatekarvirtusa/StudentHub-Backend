using System.Globalization;
using Data.Models;
using FluentValidation;

namespace API.Validators
{
    public class StudentDetailValidator : AbstractValidator<StudentDetail>
    {
        public StudentDetailValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(150).WithMessage("Name must not exceed 150 characters.");

            RuleFor(x => x.MobileNo)
                .Matches(@"^\d{10}$").WithMessage("Mobile number must be exactly 10 digits.")
                .When(x => !string.IsNullOrWhiteSpace(x.MobileNo));

            RuleFor(x => x.Dob)
                .Must(BeAValidPastDate).WithMessage("Date of birth must be a valid past date in yyyy-MM-dd format.")
                .When(x => !string.IsNullOrWhiteSpace(x.Dob));
        }

        private static bool BeAValidPastDate(string? dob)
        {
            return DateTime.TryParseExact(dob, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date)
                   && date.Date < DateTime.Today;
        }
    }
}
