using Data.Common;
using FluentValidation;

namespace API.Validators
{
    /// <summary>Rules shared by every paged list. Use it with Include(new PaginationRules()).</summary>
    public class PaginationRules : AbstractValidator<PaginationQuery>
    {
        public PaginationRules()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThanOrEqualTo(1).WithMessage("Page number must be 1 or more.");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, PaginationQuery.MaxPageSize)
                .WithMessage($"Page size must be between 1 and {PaginationQuery.MaxPageSize}.");
        }
    }
}
