using FluentValidation;
using JobBoard.Recruitment.Domain.Requests.Jobs;

namespace JobBoard.Recruitment.Application.Validators.Jobs;

public class CreateJobValidator : AbstractValidator<CreateJobRequest>
{
    public CreateJobValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Job title is required.")
            .MinimumLength(5).WithMessage("Job title must be at least 5 characters long.")
            .MaximumLength(200).WithMessage("Job title cannot exceed 200 characters.");
        
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Job description is required.")
            .MinimumLength(20).WithMessage("Job description must be informative (minimum 20 characters).")
            .MaximumLength(4000).WithMessage("Job description is too long (maximum 4000 characters).");
        
        RuleFor(x => x.Requirements)
            .NotEmpty().WithMessage("Candidate requirements are required.")
            .MaximumLength(4000).WithMessage("Requirements text is too long.")
            .When(x => !string.IsNullOrEmpty(x.Requirements));
    }
}