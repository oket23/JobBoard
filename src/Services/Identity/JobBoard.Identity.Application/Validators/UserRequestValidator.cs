using FluentValidation;
using JobBoard.Identity.Domain.Requests.Users;

namespace JobBoard.Identity.Application.Validators;

public class UserRequestValidator : AbstractValidator<UserRequest>
{
    public UserRequestValidator()
    {
        Include(new PaginationValidator());
        
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Id must be greater than 0")
            .When(x => x.Id.HasValue);
        
        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Invalid email format")
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.Email));
        
        RuleFor(x => x.Role)
            .IsInEnum().WithMessage("Invalid role value")
            .When(x => x.Role.HasValue);
        
        RuleFor(x => x.Gender)
            .IsInEnum().WithMessage("Invalid gender value")
            .When(x => x.Gender.HasValue);
        
        RuleFor(x => x.BornTo)
            .GreaterThanOrEqualTo(x => x.BornFrom)
            .WithMessage("'Born To' date must be greater than or equal to 'Born From' date.")
            .When(x => x.BornFrom.HasValue && x.BornTo.HasValue);
        
        RuleFor(x => x.CreatedTo)
            .GreaterThanOrEqualTo(x => x.CreatedFrom)
            .WithMessage("'Created To' date must be greater than or equal to 'Created From' date.")
            .When(x => x.CreatedFrom.HasValue && x.CreatedTo.HasValue);
        
        RuleFor(x => x.CreatedFrom)
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Created date cannot be in the future")
            .When(x => x.CreatedFrom.HasValue);
    }
}