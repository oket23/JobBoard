using FluentValidation;
using JobBoard.Identity.Domain.Requests.Auth;

namespace JobBoard.Identity.Application.Validators;

public class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
{
    public RegisterUserRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format")
            .Matches(@"^[^@\s,]+@[^@\s,]+\.[^@\s,]+$").WithMessage("Email contains invalid characters (like commas)")
            .MaximumLength(100);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter")
            .Matches("[0-9]").WithMessage("Password must contain at least one digit");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(50)
            .Matches(@"^[\p{L}]+$").WithMessage("First name must contain only letters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(50)
            .Matches(@"^[\p{L}]+$").WithMessage("Last name must contain only letters");
        
        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth is required")
            .LessThan(DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Date of birth cannot be in the future")
            .Must(BeAtLeast3YearsOld).WithMessage("User must be at least 3 years old");
        
        RuleFor(x => x.Gender)
            .IsInEnum().WithMessage("Invalid gender value. Allowed values are: Male, Female, NotSpecified");
    }
    
    private bool BeAtLeast3YearsOld(DateOnly dateOfBirth)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        return dateOfBirth <= today.AddYears(-3);
    }
}