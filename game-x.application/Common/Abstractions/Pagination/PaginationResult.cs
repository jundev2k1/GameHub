namespace game_x.application.Common.Abstractions.Pagination;

public class PaginationResult<TModel>(
    IEnumerable<TModel> items, int totalItems, int totalPages, int pageIndex, int pageSize)
    where TModel : class
{
    public IEnumerable<TModel> Items { get; } = items;
    public int TotalItems { get; } = totalItems;
    public int TotalPages { get; } = totalPages;
    public int PageSize { get; } = pageSize;
    public int PageNumber { get; } = pageIndex;
}
