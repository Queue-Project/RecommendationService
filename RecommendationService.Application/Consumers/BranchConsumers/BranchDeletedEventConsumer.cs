using System.Net;
using BranchService.Contracts.Events.BranchEvents;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RecommendationService.Application.Exceptions;
using RecommendationService.Application.Interfaces;

namespace RecommendationService.Application.Consumers.BranchConsumers;

public class BranchDeletedEventConsumer : IConsumer<BranchDeletedEvent>
{
    private readonly ILogger<BranchDeletedEventConsumer> _logger;
    private readonly IRecommendationDbContext _dbContext;

    public BranchDeletedEventConsumer(ILogger<BranchDeletedEventConsumer> logger, IRecommendationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<BranchDeletedEvent> context)
    {
        var request = context.Message;

        _logger.LogInformation("Deleting branch recommendation entity for BranchId {BranchId}", request.BranchId);

        var branchRecommendation =
            await _dbContext.BranchRecommendations.FirstOrDefaultAsync(s => s.BranchId == request.BranchId);

        if (branchRecommendation== null)
        {
            _logger.LogError("Branch with Id {BranchId} not found in Branch Recommendation Entity" ,request.BranchId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound,
                $"Branch with Id {request.BranchId} not found in Branch Recommendation Entity");
        }


        _dbContext.BranchRecommendations.Remove(branchRecommendation);
        await _dbContext.SaveChangesAsync(CancellationToken.None);
        

        _logger.LogInformation("Deleted branch recommendation entity with Id {RecommendationId}",
            branchRecommendation.Id);
    }
}