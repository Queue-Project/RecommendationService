using BranchService.Contracts.Events.CompanyEvents;
using MassTransit;
using Microsoft.Extensions.Logging;
using RecommendationService.Application.Interfaces;
using RecommendationService.Domain.Models;

namespace RecommendationService.Application.Consumers.CompanyConsumers;

public class CompanyCreatedEventConsumer : IConsumer<CompanyCreatedEvent>
{
    private readonly ILogger<CompanyCreatedEventConsumer> _logger;
    private readonly IRecommendationDbContext _dbContext;

    public CompanyCreatedEventConsumer(ILogger<CompanyCreatedEventConsumer> logger, IRecommendationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<CompanyCreatedEvent> context)
    {
        var request = context.Message;

        _logger.LogInformation("Creating company recommendation entity for CompanyId {CompanyId}", request.CompanyId);

        var companyRecommendation = new CompanyRecommendationEntity
        {
            CompanyId = request.CompanyId,
            CategoryId = (int)request.CompanyCategory,
            CreatedAt = DateTimeOffset.UtcNow,
            RecommendationScore = 0,
            AverageRating = 0,
            CompletedQueues = 0,
            CancelledQueues = 0,
            ComplaintCount = 0,
        };

        await _dbContext.CompanyRecommendations.AddAsync(companyRecommendation, context.CancellationToken);
        await _dbContext.SaveChangesAsync(context.CancellationToken);

        _logger.LogInformation("Created company recommendation entity with Id {RecommendationId}",
            companyRecommendation.Id);
    }
}