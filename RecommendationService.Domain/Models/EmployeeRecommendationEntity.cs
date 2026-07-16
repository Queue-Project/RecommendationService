namespace RecommendationService.Domain.Models;

public class EmployeeRecommendationEntity: BaseRecommendationEntity
{
    public int EmployeeId { get; set; }

    public int ServiceId { get; set; }

    public int BranchId { get; set; }

    public int CompanyId { get; set; }

    public int CategoryId { get; set; }

}