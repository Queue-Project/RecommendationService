using Microsoft.EntityFrameworkCore;
using RecommendationService.Infrastructure.Persistence.Database;

namespace RecommendationService.UnitTests.RecommendationService.Application.Tests.Infrastructure;

public static class TestDbContextFactory
{
    public static RecommendationDbContext Create()
    {
        var options = new DbContextOptionsBuilder<RecommendationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new RecommendationDbContext(options);

        context.Database.EnsureCreated();

        return context;
    }
}