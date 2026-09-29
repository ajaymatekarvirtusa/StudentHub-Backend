using Data.Models.Masters;
using FluentValidation;

namespace API.Validators.Masters
{
    public class CountryRequestValidator : AbstractValidator<CountryRequest>
    {
        public CountryRequestValidator()
        {
            RuleFor(x => x.Name).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Country name is required.")
                .MaximumLength(100).WithMessage("Country name must not exceed 100 characters.")
                .Matches(@"^[\p{L} .'\-()]+$").WithMessage("Country name can contain only letters, spaces and . ' - ( )");

            RuleFor(x => x.Code).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Country code is required.")
                .Matches("^[A-Za-z]{2,3}$").WithMessage("Country code must be 2 or 3 letters (ISO 3166), e.g. IN or IND.");
        }
    }
}
