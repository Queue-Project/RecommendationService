using MassTransit;
using Microsoft.Extensions.Logging;
using QUserService.Contracts.Events.EmployeeEvent;
using RecommendationService.Application.Interfaces;
using RecommendationService.Domain.Models;

namespace RecommendationService.Application.Consumers.EmployeeConsumers;

public class EmployeeCreatedEventConsumer : IConsumer<EmployeeCreatedEvent>
{
    private readonly ILogger<EmployeeCreatedEventConsumer> _logger;
    private readonly IRecommendationDbContext _dbContext;

    public EmployeeCreatedEventConsumer(ILogger<EmployeeCreatedEventConsumer> logger,
        IRecommendationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<EmployeeCreatedEvent> context)
    {
        var request = context.Message;


        _logger.LogInformation("Creating company recommendation entity for CompanyId {CompanyId}", request.CompanyId);

        var companyRecommendation = new EmployeeRecommendationEntity()
        {
            EmployeeId = request.EmployeeId,
            ServiceId = request.ServiceId ?? 0,
            BranchId = request.BranchId ?? 0,
            CompanyId = request.CompanyId,
            CategoryId = (int)request.CompanyCategory,
            CreatedAt = DateTimeOffset.UtcNow,
            RecommendationScore = 0,
            AverageRating = 0,
            CompletedQueues = 0,
            CancelledQueues = 0,
            ComplaintCount = 0,
        };

        await _dbContext.EmployeeRecommendations.AddAsync(companyRecommendation, context.CancellationToken);
        await _dbContext.SaveChangesAsync(context.CancellationToken);

        _logger.LogInformation("Created company recommendation entity with Id {RecommendationId}",
            companyRecommendation.Id);
    }
}