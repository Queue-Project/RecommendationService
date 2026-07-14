using System.Net;
using BranchService.Contracts.Events.CompanyEvents;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RecommendationService.Application.Exceptions;
using RecommendationService.Application.Interfaces;

namespace RecommendationService.Application.Consumers.CompanyConsumers;

public class CompanyUpdatedEventConsumer : IConsumer<CompanyUpdatedEvent>
{
    private readonly ILogger<CompanyUpdatedEventConsumer> _logger;
    private readonly IRecommendationDbContext _dbContext;

    public CompanyUpdatedEventConsumer(ILogger<CompanyUpdatedEventConsumer> logger, IRecommendationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<CompanyUpdatedEvent> context)
    {
        var request = context.Message;

        _logger.LogInformation("Updating company recommendation entity for CompanyId {CompanyId}", request.CompanyId);

        var companyRecommendation =
            await _dbContext.CompanyRecommendations.FirstOrDefaultAsync(s => s.CompanyId == request.CompanyId);

        if (companyRecommendation== null)
        {
            _logger.LogError("Company with Id {CompanyId} not found in Company Recommendation Entity" ,request.CompanyId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound,
                $"Company with Id {request.CompanyId} not found in Company Recommendation Entity");
        }
        
        companyRecommendation.CategoryId = (int)request.CompanyCategory;

        await _dbContext.SaveChangesAsync(context.CancellationToken);

        _logger.LogInformation("Updated company recommendation entity with Id {RecommendationId}",
            companyRecommendation.Id);
    }
}