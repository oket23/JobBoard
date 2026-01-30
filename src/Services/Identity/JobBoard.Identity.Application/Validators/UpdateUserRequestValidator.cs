using FluentValidation;
using JobBoard.Identity.Domain.Requests.Users;

namespace JobBoard.Identity.Application.Validators;

public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .MaximumLength(50)
            .When(x => x.FirstName is not null);

        RuleFor(x => x.LastName)
            .MaximumLength(50)
            .When(x => x.LastName is not null);
        
        RuleFor(x => x.DateOfBirth)
            .LessThan(DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Date of birth cannot be in the future")
            .Must(BeAtLeast3YearsOld)
            .WithMessage("User must be at least 3 years old")
            .When(x => x.DateOfBirth.HasValue);
        
        RuleFor(x => x.Gender)
            .IsInEnum().WithMessage("Invalid gender value")
            .When(x => x.Gender.HasValue);
    }

    private bool BeAtLeast3YearsOld(DateOnly? dateOfBirth)
    {
        if (!dateOfBirth.HasValue)
        {
            return true;
        }
        
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var minAllowedDate = today.AddYears(-3);

        return dateOfBirth <= minAllowedDate;
    }
}