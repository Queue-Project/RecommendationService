using MessagePack;

namespace RecommendationService.Contracts.Requests;

[MessagePackObject]
public class GetRecommendedBranchesRequest
{
    [Key(0)] public int CompanyId { get; set; }
    [Key(1)] public int PageNumber { get; set; }
    [Key(2)] public int PageSize { get; set; }
}