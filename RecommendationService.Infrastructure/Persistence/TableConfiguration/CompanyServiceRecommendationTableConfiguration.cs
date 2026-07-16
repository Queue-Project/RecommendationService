using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecommendationService.Domain.Models;

namespace RecommendationService.Infrastructure.Persistence.TableConfiguration;

public class CompanyServiceRecommendationTableConfiguration: IEntityTypeConfiguration<CompanyServiceRecommendationEntity>
{
    public void Configure(EntityTypeBuilder<CompanyServiceRecommendationEntity> builder)
    {
        builder.ToTable("CompanyServiceRecommendations");
        builder.HasKey(s => s.Id);
        builder.HasIndex(s => s.CompanyId);
        builder.HasIndex(s => s.CategoryId);
        builder.HasIndex(s => s.BranchId);
        builder.HasIndex(s => s.ServiceId).IsUnique();
    }
}