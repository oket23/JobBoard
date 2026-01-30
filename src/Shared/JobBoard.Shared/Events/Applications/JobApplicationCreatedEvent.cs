namespace JobBoard.Shared.Events.Applications;

public record JobApplicationCreatedEvent(
    int ApplicationId,
    string JobTitle,
    string CandidateEmail,
    string CandidateFirstName 
);