using FluentValidation;
using JobBoard.Recruitment.Domain.Requests.Applications;

namespace JobBoard.Recruitment.Application.Validators.Applications;

public class CreateApplicationValidator : AbstractValidator<CreateApplicationRequest>
{
    public CreateApplicationValidator()
    {
        RuleFor(x => x.CoverLetter)
            .NotEmpty()
            .MaximumLength(4000);
    }
}