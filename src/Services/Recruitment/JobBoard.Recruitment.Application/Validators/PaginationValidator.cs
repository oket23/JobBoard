using FluentValidation;
using JobBoard.Recruitment.Domain;

namespace JobBoard.Recruitment.Application.Validators;

public class PaginationValidator : AbstractValidator<PaginationBase>
{
    public PaginationValidator()
    {
        RuleFor(x => x.Offset)
            .GreaterThanOrEqualTo(0).WithMessage("Offset cannot be negative");

        RuleFor(x => x.Limit)
            .GreaterThan(0).WithMessage("Limit must be greater than 0")
            .LessThanOrEqualTo(100).WithMessage("Page size cannot exceed 100 items");
    }
}