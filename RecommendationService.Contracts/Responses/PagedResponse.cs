using MessagePack;

namespace RecommendationService.Contracts.Responses;

[MessagePackObject]
public class PagedResponse<T>
{
   [Key(0)] public List<T> Items { get; set; } = new();
   [Key(1)] public int PageNumber { get; set; }
   [Key(2)] public int PageSize { get; set; }
   [Key(3)] public int TotalCount { get; set; }
   [Key(4)] public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;
   [Key(5)] public bool HasPreviousPage => PageNumber > 1;
   [Key(6)] public bool HasNextPage => PageNumber < TotalPages;
}