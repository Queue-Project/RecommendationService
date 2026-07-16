using MessagePack;

namespace RecommendationService.Contracts.Responses;

[MessagePackObject]
public class RecommendedBranchResponse
{
    [Key(0)] public int BranchId { get; set; }
    [Key(1)] public int CompanyId { get; set; }
    [Key(2)] public int CategoryId { get; set; }
    [Key(3)] public double RecommendationScore { get; set; }
    [Key(4)] public double AverageRating { get; set; }
    [Key(5)] public int ReviewCount { get; set; }
    [Key(6)] public int CompletedQueues { get; set; }
    [Key(7)] public int ComplaintCount { get; set; }
    [Key(8)] public DateTimeOffset UpdatedAt { get; set; }
    
    
    
}