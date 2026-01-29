using FluentValidation;
using JobBoard.Recruitment.Domain.Requests.Jobs;

namespace JobBoard.Recruitment.Application.Validators.Jobs;

public class UpdateJobValidator : AbstractValidator<UpdateJobRequest>
{
    public UpdateJobValidator()
    {
        RuleFor(x => x.Title)
            .MinimumLength(5).WithMessage("Job title must be at least 5 characters long.")
            .MaximumLength(200).WithMessage("Job title cannot exceed 200 characters.")
            .When(x => !string.IsNullOrEmpty(x.Title));
        
        RuleFor(x => x.Description)
            .MinimumLength(20).WithMessage("Job description must be informative (minimum 20 characters).")
            .MaximumLength(4000).WithMessage("Job description is too long (maximum 4000 characters).")
            .When(x => !string.IsNullOrEmpty(x.Description));
        
        RuleFor(x => x.Requirements)
            .MaximumLength(4000).WithMessage("Requirements text is too long.")
            .When(x => !string.IsNullOrEmpty(x.Requirements));
    }
}