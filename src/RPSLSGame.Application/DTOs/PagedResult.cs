namespace RPSLSGame.Application.DTOs;

public class PagedResult<T>(List<T> items, int totalCount, int page, int pageSize)
{
    public List<T> Items { get; } = items ?? [];
    public int TotalCount { get; } = totalCount;
    public int Page { get; } = page;
    public int PageSize { get; } = pageSize;
    public bool HasMore => (Page * PageSize) < TotalCount;
}
