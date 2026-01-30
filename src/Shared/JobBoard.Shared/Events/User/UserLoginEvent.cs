namespace JobBoard.Shared.Events.User;

public record UserLoginEvent(
    int UserId, 
    string Email, 
    string FirstName, 
    DateTime Timestamp
);