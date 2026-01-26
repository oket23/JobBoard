namespace JobBoard.Identity.Domain.Requests.Users;

public class GetUsersBatchRequest : PaginationBase
{
    public required IEnumerable<int> Ids { get; set; }
}