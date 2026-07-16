using RecommendationService.Domain.Models;

namespace RecommendationService.UnitTests.RecommendationService.Application.Tests.Infrastructure;

public class TestDataSeeder
{
    public static CompanyRecommendationEntity CreateCompany()
    {
        return new CompanyRecommendationEntity
        {
            Id = 1,
            CompanyId = 1,
            CategoryId = 1,
            RecommendationScore = 0,
            AverageRating = 0,
            ReviewCount = 0,
            CancelledQueues = 0,
            CompletedQueues = 0,
            ComplaintCount = 0,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    public static BranchRecommendationEntity CreateBranch()
    {
        return new BranchRecommendationEntity()
        {
            Id = 1,
            BranchId = 1,
            CompanyId = 1,
            CategoryId = 1,
            RecommendationScore = 0,
            AverageRating = 0,
            ReviewCount = 0,
            CancelledQueues = 0,
            CompletedQueues = 0,
            ComplaintCount = 0,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    public static CompanyServiceRecommendationEntity CreateCompanyService()
    {
        return new CompanyServiceRecommendationEntity()
        {
            Id = 1,
            ServiceId = 1,
            BranchId = 1,
            CompanyId = 1,
            CategoryId = 1,
            RecommendationScore = 0,
            AverageRating = 0,
            ReviewCount = 0,
            CancelledQueues = 0,
            CompletedQueues = 0,
            ComplaintCount = 0,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    public static EmployeeRecommendationEntity CreateEmployee()
    {
        return new EmployeeRecommendationEntity()
        {
            Id = 1,
            EmployeeId = 1,
            ServiceId = 1,
            BranchId = 1,
            CompanyId = 1,
            CategoryId = 1,
            RecommendationScore = 0,
            AverageRating = 0,
            ReviewCount = 0,
            CancelledQueues = 0,
            CompletedQueues = 0,
            ComplaintCount = 0,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }
}