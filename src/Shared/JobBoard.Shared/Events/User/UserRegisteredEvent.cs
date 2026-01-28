namespace JobBoard.Shared.Events.User;

public record UserRegisteredEvent(
    int UserId, 
    string Email, 
    string FirstName);