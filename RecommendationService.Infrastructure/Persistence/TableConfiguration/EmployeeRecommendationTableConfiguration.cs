using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RecommendationService.Domain.Models;

namespace RecommendationService.Infrastructure.Persistence.TableConfiguration;

public class EmployeeRecommendationTableConfiguration : IEntityTypeConfiguration<EmployeeRecommendationEntity>
{
    public void Configure(EntityTypeBuilder<EmployeeRecommendationEntity> builder)
    {
        builder.ToTable("EmployeeRecommendations");
        builder.HasKey(s => s.Id);
        builder.HasIndex(s => s.CompanyId);
        builder.HasIndex(s => s.CategoryId);
        builder.HasIndex(s => s.BranchId);
        builder.HasIndex(s => s.ServiceId);
        builder.HasIndex(s => s.EmployeeId)
            .IsUnique();
    }
}