using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using QAuditLogService.Contracts;
using QContracts.Events;
using QContracts.Events.Enums;
using RecommendationService.Application.Consumers.QueueEventConsumers;
using RecommendationService.Application.Interfaces;
using RecommendationService.UnitTests.RecommendationService.Application.Tests.Infrastructure;
using Shouldly;

namespace RecommendationService.UnitTests.RecommendationService.Application.Tests.ConsumerTests.QueueConsumersTests;

public class QueueCancelledEventConsumerTests
{
    private readonly Mock<ILogger<QueueCancelledEventConsumer>> _mockLogger;
    private readonly Mock<ConsumeContext<QueueEvent>> _mockContext;
    private readonly IRecommendationDbContext _context;
    private readonly Mock<IRecommendationScoreService> _mockScoreService;
    private readonly QueueCancelledEventConsumer _consumer;

    public QueueCancelledEventConsumerTests()
    {
        _mockLogger = new Mock<ILogger<QueueCancelledEventConsumer>>();
        _mockContext = new Mock<ConsumeContext<QueueEvent>>();
        _context = TestDbContextFactory.Create();
        _mockScoreService = new Mock<IRecommendationScoreService>();
        _consumer = new QueueCancelledEventConsumer(_mockLogger.Object, _context, _mockScoreService.Object);
    }

    [Fact]
    public async Task Consume_Should_Update_Data_When_Review_Created_Event_Received()
    {
        //Arrange
        var company = TestDataSeeder.CreateCompany();
        var branch = TestDataSeeder.CreateBranch();
        var service = TestDataSeeder.CreateCompanyService();
        var employee = TestDataSeeder.CreateEmployee();
        await _context.CompanyRecommendations.AddAsync(company, CancellationToken.None);
        await _context.BranchRecommendations.AddAsync(branch, CancellationToken.None);
        await _context.ServiceRecommendations.AddAsync(service, CancellationToken.None);
        await _context.EmployeeRecommendations.AddAsync(employee, CancellationToken.None);
        await _context.SaveChangesAsync(CancellationToken.None);


        _mockScoreService.Setup(s => s.CalculateRecommendationScore(company)).Returns(1);
        _mockScoreService.Setup(s => s.CalculateRecommendationScore(service)).Returns(1);
        _mockScoreService.Setup(s => s.CalculateRecommendationScore(branch)).Returns(1);
        _mockScoreService.Setup(s => s.CalculateRecommendationScore(employee)).Returns(1);

        var occurredAt = DateTime.UtcNow;


        var expectedEvent = new QueueEvent()
        {
            Email = "test@gmail.com",
            EmployeeId = 1,
            CompanyId = 1,
            BranchId = 1,
            ServiceId = 1,
            QueueId = 1,
            CustomerId = 1,
            EventType = QueueEventType.Updated,
            Status = UpdatedQueueStatus.CanceledByCustomer,
            StartTime = DateTimeOffset.UtcNow,
            EndTime = DateTimeOffset.UtcNow.AddMinutes(30),
            CancelReason = "test cancel",
            OccurredAt = occurredAt,
            AuditData = new AuditData
            {
                PerformedByUserId = 1,
                PerformedByUserName = "systemAdmin",
                Changes = new List<AuditEventLogDetails>()
            }
        };

        _mockContext.Setup(s => s.Message).Returns(expectedEvent);
        _mockContext.Setup(s => s.CancellationToken).Returns(CancellationToken.None);

        //Act

        await _consumer.Consume(_mockContext.Object);

        //Assert

        var savedCompany = await _context.CompanyRecommendations.FirstOrDefaultAsync();
        savedCompany!.CancelledQueues.ShouldBe(1);
        savedCompany.RecommendationScore.ShouldNotBe(0);

        var savedBranch = await _context.BranchRecommendations.FirstOrDefaultAsync();
        savedBranch!.CancelledQueues.ShouldBe(1);
        savedBranch.RecommendationScore.ShouldNotBe(0);

        var savedService = await _context.ServiceRecommendations.FirstOrDefaultAsync();
        savedService!.CancelledQueues.ShouldBe(1);
        savedService.RecommendationScore.ShouldNotBe(0);

        var savedEmployee = await _context.ServiceRecommendations.FirstOrDefaultAsync();
        savedEmployee!.CancelledQueues.ShouldBe(1);
        savedEmployee.RecommendationScore.ShouldNotBe(0);
    }
}