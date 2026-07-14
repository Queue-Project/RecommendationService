using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecommendationService.Domain.Models;

namespace RecommendationService.Infrastructure.Persistence.TableConfiguration;

public class CompanyRecommendationTableConfiguration: IEntityTypeConfiguration<CompanyRecommendationEntity>
{
    public void Configure(EntityTypeBuilder<CompanyRecommendationEntity> builder)
    {
        builder.ToTable("CompanyRecommendations");
        builder.HasKey(s => s.Id);
        builder.HasIndex(s => s.CompanyId).IsUnique();
        builder.HasIndex(s => s.CategoryId);
    }
}