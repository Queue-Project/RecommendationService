namespace RecommendationService.Domain.Models;

public class CompanyServiceRecommendationEntity: BaseRecommendationEntity
{
    public int ServiceId { get; set; }

    public int BranchId { get; set; }

    public int CompanyId { get; set; }

    public int CategoryId { get; set; }

}