using BranchService.Contracts.Events.CompanyServiceEvents;
using MassTransit;
using Microsoft.Extensions.Logging;
using RecommendationService.Application.Interfaces;
using RecommendationService.Domain.Models;

namespace RecommendationService.Application.Consumers.CompanyServiceConsumers;

public class CompanyServiceCreatedEventConsumer : IConsumer<CompanyServiceCreatedEvent>
{
    private readonly ILogger<CompanyServiceCreatedEventConsumer> _logger;
    private readonly IRecommendationDbContext _dbContext;

    public CompanyServiceCreatedEventConsumer(ILogger<CompanyServiceCreatedEventConsumer> logger, IRecommendationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<CompanyServiceCreatedEvent> context)
    {
        var request = context.Message;

        _logger.LogInformation("Creating company service recommendation entity for CompanyServiceId {CompanyServiceId}", request.CompanyServiceId);

        var companyServiceRecommendation = new CompanyServiceRecommendationEntity
        {
            ServiceId   = request.CompanyServiceId,
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

        await _dbContext.ServiceRecommendations.AddAsync(companyServiceRecommendation, context.CancellationToken);
        await _dbContext.SaveChangesAsync(context.CancellationToken);

        _logger.LogInformation("Created company service recommendation entity with Id {RecommendationId}",
            companyServiceRecommendation.Id);
    }
}