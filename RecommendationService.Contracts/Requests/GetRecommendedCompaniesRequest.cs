using MessagePack;

namespace RecommendationService.Contracts.Requests;

[MessagePackObject]
public class GetRecommendedCompaniesRequest
{
    [Key(0)] public int CategoryId { get; set; }
    [Key(1)] public int PageNumber { get; set; }
    [Key(2)] public int PageSize { get; set; }
    
}