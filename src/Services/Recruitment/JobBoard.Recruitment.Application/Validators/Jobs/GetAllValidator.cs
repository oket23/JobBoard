using FluentValidation;
using JobBoard.Recruitment.Domain.Requests.Jobs;

namespace JobBoard.Recruitment.Application.Validators.Jobs;

public class GetAllValidator : AbstractValidator<JobRequest>
{
    public GetAllValidator()
    {
        Include(new PaginationValidator());
        
        RuleFor(x => x.Title)
            .MinimumLength(5).WithMessage("Job title must be at least 5 characters long.")
            .MaximumLength(200).WithMessage("Job title cannot exceed 200 characters.")
            .When(x => !string.IsNullOrEmpty(x.Title));
        
        RuleFor(x => x.Description)
            .MaximumLength(50).WithMessage("Job description is too long.")
            .When(x => !string.IsNullOrEmpty(x.Description));
        
        RuleFor(x => x.Requirements)
            .MaximumLength(50).WithMessage("Requirements text is too long.")
            .When(x => !string.IsNullOrEmpty(x.Requirements));
    }
}