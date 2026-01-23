using FluentValidation;
using JobBoard.Identity.Domain.Requests.Users;

namespace JobBoard.Identity.Application.Validators;

public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().EmailAddress();

        RuleFor(x => x.FirstName)
            .NotEmpty().MaximumLength(50);

        RuleFor(x => x.LastName)
            .NotEmpty().MaximumLength(50);
        
        RuleFor(x => x.DateOfBirth)
            .LessThan(DateTime.UtcNow).WithMessage("Date of birth cannot be in the future")
            .Must(BeAtLeast3YearsOld).WithMessage("User must be at least 3 years old");
        
        RuleFor(x => x.Gender)
            .IsInEnum().WithMessage("Invalid gender value");
    }

    private bool BeAtLeast3YearsOld(DateTime dateOfBirth)
    {
        return dateOfBirth <= DateTime.UtcNow.AddYears(-3);
    }
}