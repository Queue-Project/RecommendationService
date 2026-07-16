using System.Net;
using BranchService.Contracts.Events.CompanyEvents;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RecommendationService.Application.Exceptions;
using RecommendationService.Application.Interfaces;

namespace RecommendationService.Application.Consumers.CompanyConsumers;

public class CompanyDeletedEventConsumer : IConsumer<CompanyDeletedEvent>
{
    private readonly ILogger<CompanyDeletedEventConsumer> _logger;
    private readonly IRecommendationDbContext _dbContext;

    public CompanyDeletedEventConsumer(ILogger<CompanyDeletedEventConsumer> logger, IRecommendationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<CompanyDeletedEvent> context)
    {
        var request = context.Message;

        _logger.LogInformation("Deleting company recommendation entity for CompanyId {CompanyId}", request.CompanyId);

        var companyRecommendation =
            await _dbContext.CompanyRecommendations.FirstOrDefaultAsync(s => s.CompanyId == request.CompanyId);

        if (companyRecommendation== null)
        {
            _logger.LogError("Company with Id {CompanyId} not found in Company Recommendation Entity" ,request.CompanyId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound,
                $"Company with Id {request.CompanyId} not found in Company Recommendation Entity");
        }


        _dbContext.CompanyRecommendations.Remove(companyRecommendation);
        await _dbContext.SaveChangesAsync(CancellationToken.None);

        _logger.LogInformation("Deleted company recommendation entity with Id {RecommendationId}",
            companyRecommendation.Id);
    }
}