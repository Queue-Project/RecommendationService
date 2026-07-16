using Microsoft.EntityFrameworkCore;
using RecommendationService.Application.Interfaces;
using RecommendationService.Domain.Models;

namespace RecommendationService.Infrastructure.Persistence.Database;

public class RecommendationDbContext: DbContext, IRecommendationDbContext
{

    public RecommendationDbContext(DbContextOptions<RecommendationDbContext> options): base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Ignore<BaseRecommendationEntity>();
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CompanyRecommendationEntity).Assembly);
        base.OnModelCreating(modelBuilder);
    }
    
    
    public DbSet<CompanyRecommendationEntity> CompanyRecommendations { get; set; }
    public DbSet<BranchRecommendationEntity> BranchRecommendations { get; set; }
    public DbSet<CompanyServiceRecommendationEntity> ServiceRecommendations { get; set; }
    public DbSet<EmployeeRecommendationEntity> EmployeeRecommendations { get; set; }
}