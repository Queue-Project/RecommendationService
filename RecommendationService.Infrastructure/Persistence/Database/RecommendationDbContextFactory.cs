using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace RecommendationService.Infrastructure.Persistence.Database;

public class RecommendationDbContextFactory: IDesignTimeDbContextFactory<RecommendationDbContext>
{
    public RecommendationDbContext CreateDbContext(string[] args)
    {
        var optionBuilder = new DbContextOptionsBuilder<RecommendationDbContext>();
        
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../RecommendationService.API"))
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
        
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        optionBuilder.UseNpgsql(connectionString);
        return new RecommendationDbContext(optionBuilder.Options);
    }
}