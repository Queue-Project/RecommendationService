using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QContracts.Events.ReviewEvents;
using RecommendationService.Application.Interfaces;

namespace RecommendationService.Application.Consumers.ReviewEventConsumers;

public class ReviewCreatedEventConsumer: IConsumer<ReviewCreatedEvent>
{
    private readonly ILogger<ReviewCreatedEventConsumer> _logger;
    private readonly IRecommendationDbContext _dbContext;
    private readonly IRecommendationScoreService _scoreService;

    public ReviewCreatedEventConsumer(ILogger<ReviewCreatedEventConsumer> logger, IRecommendationDbContext dbContext, IRecommendationScoreService scoreService)
    {
        _logger = logger;
        _dbContext = dbContext;
        _scoreService = scoreService;
    }

    public async Task Consume(ConsumeContext<ReviewCreatedEvent> context)
    {
        var request = context.Message;
        
        _logger.LogInformation(
            "Processing review created event. ReviewId: {ReviewId} QueueId: {QueueId}, CompanyId: {CompanyId}, EmployeeId: {EmployeeId}",
            request.ReviewId,
            request.QueueId,
            request.CompanyId,
            request.EmployeeId);


        var company =
            await _dbContext.CompanyRecommendations
                .FirstOrDefaultAsync(
                    x => x.CompanyId == request.CompanyId,
                    context.CancellationToken);


        var branch =
            await _dbContext.BranchRecommendations
                .FirstOrDefaultAsync(
                    x =>
                        x.BranchId == request.BranchId &&
                        x.CompanyId == request.CompanyId,
                    context.CancellationToken);


        var service =
            await _dbContext.ServiceRecommendations
                .FirstOrDefaultAsync(
                    x =>
                        x.ServiceId == request.ServiceId &&
                        x.CompanyId == request.CompanyId &&
                        x.BranchId == request.BranchId,
                    context.CancellationToken);


        var employee =
            await _dbContext.EmployeeRecommendations
                .FirstOrDefaultAsync(
                    x =>
                        x.EmployeeId == request.EmployeeId &&
                        x.CompanyId == request.CompanyId &&
                        x.BranchId == request.BranchId &&
                        x.ServiceId == request.ServiceId,
                    context.CancellationToken);



        if (company != null)
        {
            var oldReviewCount = company.ReviewCount;
            var oldAverage = company.AverageRating;
            company.ReviewCount++;
            var newAverage = (oldAverage * oldReviewCount + request.Grade) / company.ReviewCount;
            company.AverageRating = newAverage;
            company.RecommendationScore =
                _scoreService.CalculateRecommendationScore(company);
            company.UpdatedAt= DateTimeOffset.UtcNow;

            _logger.LogInformation(
                "Updated company recommendation. CompanyId: {CompanyId}, Score: {Score}",
                company.CompanyId,
                company.RecommendationScore);
        }
        else
        {
            _logger.LogWarning(
                "Company recommendation not found. CompanyId: {CompanyId}",
                request.CompanyId);
        }



        if (branch != null)
        {
            
            var oldReviewCount = branch.ReviewCount;
            var oldAverage = branch.AverageRating;
            branch.ReviewCount++;
            var newAverage = (oldAverage * oldReviewCount + request.Grade) / branch.ReviewCount;
            branch.AverageRating = newAverage;
            branch.RecommendationScore =
                _scoreService.CalculateRecommendationScore(branch);
            branch.UpdatedAt= DateTimeOffset.UtcNow;

            _logger.LogInformation(
                "Updated branch recommendation. BranchId: {BranchId}, Score: {Score}",
                branch.BranchId,
                branch.RecommendationScore);
        }
        else
        {
            _logger.LogWarning(
                "Branch recommendation not found. BranchId: {BranchId}",
                request.BranchId);
        }



        if (service != null)
        {
            var oldReviewCount = service.ReviewCount;
            var oldAverage = service.AverageRating;
            service.ReviewCount++;
            var newAverage = (oldAverage * oldReviewCount + request.Grade) / service.ReviewCount;
            service.AverageRating = newAverage;
            service.RecommendationScore =
                _scoreService.CalculateRecommendationScore(service);
            service.UpdatedAt= DateTimeOffset.UtcNow;

            _logger.LogInformation(
                "Updated service recommendation. ServiceId: {ServiceId}, Score: {Score}",
                service.ServiceId,
                service.RecommendationScore);
        }
        else
        {
            _logger.LogWarning(
                "Service recommendation not found. ServiceId: {ServiceId}",
                request.ServiceId);
        }



        if (employee != null)
        {
            var oldReviewCount = employee.ReviewCount;
            var oldAverage = employee.AverageRating;
            employee.ReviewCount++;
            var newAverage = (oldAverage * oldReviewCount + request.Grade) / employee.ReviewCount;
            employee.AverageRating = newAverage;
            employee.RecommendationScore =
                _scoreService.CalculateRecommendationScore(employee);
            employee.UpdatedAt= DateTimeOffset.UtcNow;

            _logger.LogInformation(
                "Updated employee recommendation. EmployeeId: {EmployeeId}, Score: {Score}",
                employee.EmployeeId,
                employee.RecommendationScore);
        }
        else
        {
            _logger.LogWarning(
                "Employee recommendation not found. EmployeeId: {EmployeeId}",
                request.EmployeeId);
        }



        await _dbContext.SaveChangesAsync(context.CancellationToken);


        _logger.LogInformation(
            "Review  recommendation update finished. ReviewId: {ReviewId}",
            request.ReviewId);
    }
}