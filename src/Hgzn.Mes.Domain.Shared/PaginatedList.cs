namespace Hgzn.Mes.Domain.Shared
{
    public class PaginatedList<T>
    {
        public PaginatedList(IEnumerable<T> items, long totalCount, long pageIndex, int pageSize)
        {
            Items = items;
            TotalCount = totalCount; PageIndex = pageIndex; PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        }

        public long TotalCount { get; init; }
        public long PageIndex { get; init; }
        public int PageSize { get; init; }
        public long TotalPages { get; init; }
        public IEnumerable<T> Items { get; init; }
    }
}