using MessagePack;

namespace RecommendationService.Contracts.Requests;

[MessagePackObject]
public class GetRecommendedServicesRequest
{
    [Key(0)] public int CompanyId { get; set; }
    [Key(1)] public int BranchId { get; set; }
    [Key(2)] public int PageNumber { get; set; }
    [Key(3)] public int PageSize { get; set; }
}