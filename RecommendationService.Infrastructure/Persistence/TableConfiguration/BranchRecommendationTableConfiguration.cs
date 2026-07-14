using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecommendationService.Domain.Models;

namespace RecommendationService.Infrastructure.Persistence.TableConfiguration;

public class BranchRecommendationTableConfiguration: IEntityTypeConfiguration<BranchRecommendationEntity>
{
    public void Configure(EntityTypeBuilder<BranchRecommendationEntity> builder)
    {
        builder.ToTable("BranchRecommendations");
        builder.HasKey(s => s.Id);
        builder.HasIndex(s => s.CompanyId);
        builder.HasIndex(s => s.CategoryId);
        builder.HasIndex(s => s.BranchId).IsUnique();
    }
}