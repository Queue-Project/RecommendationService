using MessagePack;

namespace RecommendationService.Contracts.Responses;

[MessagePackObject]
public class RecommendedCompanyResponse
{
    [Key(0)] public int CompanyId { get; set; }
    [Key(1)] public int CategoryId { get; set; }
    [Key(2)] public double RecommendationScore { get; set; }
    [Key(3)] public double AverageRating { get; set; }
    [Key(4)] public int ReviewCount { get; set; }
    [Key(5)] public int CompletedQueues { get; set; }
    [Key(6)] public int ComplaintCount { get; set; }
    [Key(7)] public DateTimeOffset UpdatedAt { get; set; }
    
    
    
}