using FluentValidation;
using JobBoard.Identity.Domain.Requests.Users;

namespace JobBoard.Identity.Application.Validators;

public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        //RuleFor(x => x.Email).EmailAddress();

        RuleFor(x => x.FirstName)
            .MaximumLength(50)
            .When(x => x.FirstName is not null);

        RuleFor(x => x.LastName)
            .MaximumLength(50)
            .When(x => x.LastName is not null);
        
        RuleFor(x => x.DateOfBirth)
            .LessThan(DateTime.UtcNow).WithMessage("Date of birth cannot be in the future")
            .Must(BeAtLeast3YearsOld).WithMessage("User must be at least 3 years old")
            .When(x => x.DateOfBirth.HasValue);
        
        RuleFor(x => x.Gender)
            .IsInEnum().WithMessage("Invalid gender value")
            .When(x => x.Gender.HasValue);
    }

    private bool BeAtLeast3YearsOld(DateTime? dateOfBirth)
    {
        return dateOfBirth <= DateTime.UtcNow.AddYears(-3);
    }
}