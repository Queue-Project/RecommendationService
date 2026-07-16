using System.Net;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QUserService.Contracts.Events.EmployeeEvent;
using RecommendationService.Application.Exceptions;
using RecommendationService.Application.Interfaces;

namespace RecommendationService.Application.Consumers.EmployeeConsumers;

public class EmployeeDeletedEventConsumer : IConsumer<EmployeeDeletedEvent>
{
    private readonly ILogger<EmployeeDeletedEventConsumer> _logger;
    private readonly IRecommendationDbContext _dbContext;

    public EmployeeDeletedEventConsumer(ILogger<EmployeeDeletedEventConsumer> logger, IRecommendationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<EmployeeDeletedEvent> context)
    {
        var request = context.Message;

        _logger.LogInformation("Deleting employee recommendation entity for Id {Id}", request.EmployeeId);

        var employeeRecommendation =
            await _dbContext.EmployeeRecommendations.FirstOrDefaultAsync(s => s.EmployeeId == request.EmployeeId);

        if (employeeRecommendation== null)
        {
            _logger.LogError("Employee with Id {EmployeeId} not found in Employee Recommendation Entity" ,request.EmployeeId);
            throw new HttpStatusCodeException(HttpStatusCode.NotFound,
                $"Employee with Id {request.EmployeeId} not found in Employee Recommendation Entity");
        }


        _dbContext.EmployeeRecommendations.Remove(employeeRecommendation);
        await _dbContext.SaveChangesAsync(CancellationToken.None);
        

        _logger.LogInformation("Deleted company service recommendation entity with Id {RecommendationId}",
            employeeRecommendation.Id);
    }
}