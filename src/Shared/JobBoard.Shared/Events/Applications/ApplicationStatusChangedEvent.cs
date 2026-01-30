namespace JobBoard.Shared.Events.Applications;

public record ApplicationStatusChangedEvent(
    int ApplicationId,
    string JobTitle,
    string CandidateEmail,
    string CandidateFirstName, 
    string NewStatus
);