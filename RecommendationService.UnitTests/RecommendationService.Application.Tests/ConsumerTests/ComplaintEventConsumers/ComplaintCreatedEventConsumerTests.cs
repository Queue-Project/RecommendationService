using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using QAuditLogService.Contracts;
using QContracts.Enums;
using QContracts.Events;
using QContracts.Events.ComplaintEvents;
using RecommendationService.Application.Consumers.ComplaintEventConsumers;
using RecommendationService.Application.Interfaces;
using RecommendationService.UnitTests.RecommendationService.Application.Tests.Infrastructure;
using Shouldly;

namespace RecommendationService.UnitTests.RecommendationService.Application.Tests.ConsumerTests.ComplaintEventConsumers;

public class ComplaintCreatedEventConsumerTests
{
    private readonly Mock<ILogger<ComplaintCreatedEventConsumer>> _mockLogger;
    private readonly Mock<ConsumeContext<ComplaintCreatedEvent>> _mockContext;
    private readonly IRecommendationDbContext _context;
    private readonly Mock<IRecommendationScoreService> _mockScoreService;
    private readonly ComplaintCreatedEventConsumer _consumer;

    public ComplaintCreatedEventConsumerTests()
    {
        _mockLogger = new Mock<ILogger<ComplaintCreatedEventConsumer>>();
        _mockContext = new Mock<ConsumeContext<ComplaintCreatedEvent>>();
        _context = TestDbContextFactory.Create();
        _mockScoreService = new Mock<IRecommendationScoreService>();
        _consumer = new ComplaintCreatedEventConsumer(_mockLogger.Object, _context, _mockScoreService.Object);
    }

    [Fact]
    public async Task Consume_Should_Update_Data_When_Complaint_Created_Event_Received()
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


        var expectedEvent = new ComplaintCreatedEvent()
        {
            EmployeeId = 1,
            CompanyId = 1,
            BranchId = 1,
            ServiceId = 1,
            QueueId = 1,
            CustomerId = 1,
            ComplaintId = 1,
            ComplaintText = "Test Text",
            CurrentComplaintStatus = CurrentComplaintStatus.Pending,
            OccuredAt = occurredAt,
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
        savedCompany!.ComplaintCount.ShouldBe(1);
        savedCompany.RecommendationScore.ShouldNotBe(0);

        var savedBranch = await _context.BranchRecommendations.FirstOrDefaultAsync();
        savedBranch!.ComplaintCount.ShouldBe(1);
        savedBranch.RecommendationScore.ShouldNotBe(0);

        var savedService = await _context.ServiceRecommendations.FirstOrDefaultAsync();
        savedService!.ComplaintCount.ShouldBe(1);
        savedService.RecommendationScore.ShouldNotBe(0);

        var savedEmployee = await _context.ServiceRecommendations.FirstOrDefaultAsync();
        savedEmployee!.ComplaintCount.ShouldBe(1);
        savedEmployee.RecommendationScore.ShouldNotBe(0);
    }
}