using Microsoft.EntityFrameworkCore;
using RecommendationService.Domain.Models;

namespace RecommendationService.Application.Interfaces;

public interface IRecommendationDbContext
{
    DbSet<CompanyRecommendationEntity> CompanyRecommendations { get; set; }
    DbSet<BranchRecommendationEntity> BranchRecommendations { get; set; }
    DbSet<CompanyServiceRecommendationEntity> ServiceRecommendations { get; set; }
    DbSet<EmployeeRecommendationEntity> EmployeeRecommendations { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);


}