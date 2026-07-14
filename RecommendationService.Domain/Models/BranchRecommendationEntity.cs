namespace RecommendationService.Domain.Models;

public class BranchRecommendationEntity: BaseRecommendationEntity
{
    public int BranchId { get; set; }

    public int CompanyId { get; set; }

    public int CategoryId { get; set; }

}