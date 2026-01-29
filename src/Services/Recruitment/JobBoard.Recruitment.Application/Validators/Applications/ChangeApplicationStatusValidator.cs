using FluentValidation;
using JobBoard.Recruitment.Domain.Requests.Applications;

namespace JobBoard.Recruitment.Application.Validators.Applications;

public class ChangeApplicationStatusValidator : AbstractValidator<ChangeApplicationStatusRequest>
{
    public ChangeApplicationStatusValidator()
    {
        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Invalid application status.");
    }
}
