namespace JobBoard.Identity.Domain.DTOs;

public class JwtSettings
{
    public required string SecretKey { get; set; }
    public required string Issuer { get; set; } 
    public  required string Audience { get; set; }
    public int AccessExpiryMinutes { get; set; } = 60;
    public int RefreshExpiryDays { get; set; } = 7;
}