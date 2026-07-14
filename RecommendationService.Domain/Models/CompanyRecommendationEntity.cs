namespace RecommendationService.Domain.Models;

public class CompanyRecommendationEntity: BaseRecommendationEntity
{
    public int CompanyId { get; set; }

    public int CategoryId { get; set; }
}