namespace JobBoard.Recruitment.Domain.Models.Base;

public abstract class BaseEntity
{
    public DateTime? DeletedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}