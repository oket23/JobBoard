using FluentValidation;
using JobBoard.Recruitment.Domain.Requests.Applications;

namespace JobBoard.Recruitment.Application.Validators.Applications;

public class GetAllForUserValidator : AbstractValidator<UserApplicationRequest>
{
    public GetAllForUserValidator()
    {
        Include(new PaginationValidator());
        
        RuleFor(x => x.Status)
            .IsInEnum()
            .When(x => x.Status.HasValue)
            .WithMessage("Invalid application status.");
    }
}