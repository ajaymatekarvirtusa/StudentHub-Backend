using Data.Models.Masters;
using FluentValidation;

namespace API.Validators.Masters
{
    public class CountryQueryValidator : AbstractValidator<CountryQuery>
    {
        public CountryQueryValidator()
        {
            Include(new PaginationRules());

            RuleFor(x => x.Search)
                .MaximumLength(100).WithMessage("Search must not exceed 100 characters.");
        }
    }
}
