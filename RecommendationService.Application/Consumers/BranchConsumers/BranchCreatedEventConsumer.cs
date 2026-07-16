using BranchService.Contracts.Events.BranchEvents;
using MassTransit;
using Microsoft.Extensions.Logging;
using RecommendationService.Application.Interfaces;
using RecommendationService.Domain.Models;

namespace RecommendationService.Application.Consumers.BranchConsumers;

public class BranchCreatedEventConsumer : IConsumer<BranchCreatedEvent>
{
    private readonly ILogger<BranchCreatedEventConsumer> _logger;
    private readonly IRecommendationDbContext _dbContext;

    public BranchCreatedEventConsumer(ILogger<BranchCreatedEventConsumer> logger, IRecommendationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<BranchCreatedEvent> context)
    {
        var request = context.Message;

        _logger.LogInformation("Creating branch recommendation entity for BranchId {BranchId}", request.BranchId);

        var branchRecommendation = new BranchRecommendationEntity()
        {
            BranchId = request.BranchId,
            CompanyId = request.CompanyId,
            CategoryId = (int)request.CompanyCategory,
            CreatedAt = DateTimeOffset.UtcNow,
            RecommendationScore = 0,
            AverageRating = 0,
            CompletedQueues = 0,
            CancelledQueues = 0,
            ComplaintCount = 0,
        };

        await _dbContext.BranchRecommendations.AddAsync(branchRecommendation, context.CancellationToken);
        await _dbContext.SaveChangesAsync(context.CancellationToken);

        _logger.LogInformation("Created branch recommendation entity with Id {RecommendationId}",
            branchRecommendation.Id);
    }
}