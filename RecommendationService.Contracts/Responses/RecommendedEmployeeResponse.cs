using MessagePack;

namespace RecommendationService.Contracts.Responses;

[MessagePackObject]
public class RecommendedEmployeeResponse
{
    [Key(0)] public int EmployeeId { get; set; }
    [Key(1)] public int ServiceId { get; set; }
    [Key(2)] public int BranchId { get; set; }
    [Key(3)] public int CompanyId { get; set; }
    [Key(4)] public int CategoryId { get; set; }
    [Key(5)] public double RecommendationScore { get; set; }
    [Key(6)] public double AverageRating { get; set; }
    [Key(7)] public int ReviewCount { get; set; }
    [Key(8)] public int CompletedQueues { get; set; }
    [Key(9)] public int ComplaintCount { get; set; }
    [Key(10)] public DateTimeOffset UpdatedAt { get; set; }
}