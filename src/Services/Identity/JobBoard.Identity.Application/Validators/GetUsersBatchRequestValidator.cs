using FluentValidation;
using JobBoard.Identity.Domain.Requests.Users;

namespace JobBoard.Identity.Application.Validators;

public class GetUsersBatchRequestValidator : AbstractValidator<GetUsersBatchRequest>
{
    public GetUsersBatchRequestValidator()
    {
        RuleFor(x => x.Ids)
            .NotEmpty().WithMessage("The ID list cannot be empty.")
            .Must(ids => ids != null && ids.Any()).WithMessage("At least one ID must be provided.")
            .Must(ids => ids.Count() <= 100).WithMessage("You can request a maximum of 100 users per request.");
        
        RuleForEach(x => x.Ids)
            .GreaterThan(0).WithMessage("User ID must be greater than 0.");
        
        RuleFor(x => x.Limit)
            .InclusiveBetween(1, 100).WithMessage("Limit must be between 1 and 100.");

        RuleFor(x => x.Offset)
            .GreaterThanOrEqualTo(0).WithMessage("Offset cannot be negative.");
    }
}