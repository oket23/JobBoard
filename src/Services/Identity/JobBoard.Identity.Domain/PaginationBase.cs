namespace JobBoard.Identity.Domain;

public class PaginationBase
{
    public int Limit { get; set; } = 10;
    public int Offset { get; set; } = 0;
}