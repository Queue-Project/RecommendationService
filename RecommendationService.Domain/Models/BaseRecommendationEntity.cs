namespace RecommendationService.Domain.Models;

public abstract class BaseRecommendationEntity
{
    public int Id { get; set; }

    public double RecommendationScore { get; set; }

    public int CompletedQueues { get; set; }

    public int CancelledQueues { get; set; }

    public int ComplaintCount { get; set; }

    public double AverageRating { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}