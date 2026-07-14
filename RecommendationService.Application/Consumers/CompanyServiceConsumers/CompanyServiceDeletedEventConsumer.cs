using System.Net;
using BranchService.Contracts.Events.CompanyServiceEvents;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RecommendationService.Application.Exceptions;
using RecommendationService.Application.Interfaces;

namespace RecommendationService.Application.Consumers.CompanyServiceConsumers;

public class CompanyServiceDeletedEventConsumer : IConsumer<CompanyServiceDeletedEvent>
{
    private readonly ILogger<CompanyServiceDeletedEventConsumer> _logger;
    private readonly IRecommendationDbContext _dbContext;

    public CompanyServiceDeletedEventConsumer(ILogger<CompanyServiceDeletedEventConsumer> logger, IRecommendationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<CompanyServiceDeletedEvent> context)
    {
        var request = context.Message;

        _logger.LogInformation("Deleting company service recommendation entity for CompanyServiceId {CompanyServiceId}", request.CompanyServiceId);

        var companyServiceRecommendation =
            await _dbContext.ServiceRecommendations.FirstOrDefaultAsync(s => s.ServiceId == request.CompanyServiceId);

        if (companyServiceRecommendation== null)
        {
            _logger.LogError("CompanyService with Id {CompanyServiceId} not found in CompanyService Recommendation Entity" ,request.CompanyServiceId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound,
                $"CompanyService with Id {request.CompanyServiceId} not found in CompanyService Recommendation Entity");
        }


        _dbContext.ServiceRecommendations.Remove(companyServiceRecommendation);
        await _dbContext.SaveChangesAsync(CancellationToken.None);
        

        _logger.LogInformation("Deleted company service recommendation entity with Id {RecommendationId}",
            companyServiceRecommendation.Id);
    }
}