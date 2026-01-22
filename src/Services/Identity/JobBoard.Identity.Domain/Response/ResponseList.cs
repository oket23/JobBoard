namespace JobBoard.Identity.Domain.Response;

public class ResponseList<T> where T : class
{
    public IEnumerable<T> Items { get; init; } = new List<T>();
    public int TotalCount { get; init; }
    public int Limit { get; init; }
    public int Offset { get; init; }
    public int Page { get; init; }

    public ResponseList<K> ToResponseList<K>(Func<T, K> func) where K : class
    {
        return new ResponseList<K>
        {
            Items = Items.Select(func).ToList(),
            TotalCount = TotalCount,
            Limit = Limit,
            Offset = Offset,
            Page = Page
        };
    }
}