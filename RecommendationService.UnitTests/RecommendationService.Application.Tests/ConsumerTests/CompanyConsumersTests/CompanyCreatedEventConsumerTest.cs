using BranchService.Contracts.Events;
using BranchService.Contracts.Events.CompanyEvents;
using BranchService.Contracts.Events.Enums;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using QAuditLogService.Contracts;
using RecommendationService.Application.Consumers.CompanyConsumers;
using RecommendationService.Application.Interfaces;
using RecommendationService.UnitTests.RecommendationService.Application.Tests.Infrastructure;
using Shouldly;

namespace RecommendationService.UnitTests.RecommendationService.Application.Tests.ConsumerTests.CompanyConsumersTests;

public class CompanyCreatedEventConsumerTest
{
    private readonly Mock<ILogger<CompanyCreatedEventConsumer>> _mockLogger;
    private readonly IRecommendationDbContext _dbContext;
    private readonly Mock<ConsumeContext<CompanyCreatedEvent>> _mockContext;
    private readonly CompanyCreatedEventConsumer _consumer;

    public CompanyCreatedEventConsumerTest()
    {
        _mockLogger = new Mock<ILogger<CompanyCreatedEventConsumer>>();
        _dbContext = TestDbContextFactory.Create();
        _mockContext = new Mock<ConsumeContext<CompanyCreatedEvent>>();
        _consumer = new CompanyCreatedEventConsumer(_mockLogger.Object, _dbContext);
    }

    [Fact]
    public async Task Consume_Should_Create_Company_Recommendation_Entity_When_Event_Received()
    {
        //Arrange
        var occuredAt = DateTime.UtcNow;


        var expectedEvent = new CompanyCreatedEvent()
        {
            CompanyId = 1,
            CompanyCategory = CompanyCategory.Beauty,
            CompanyName = "Test Company Name",
            EmailAddress = "test@gmail.com",
            Address = "Test Address",
            PhoneNumber = "+992923324252",
            OccuredAt = occuredAt,
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
        var savedEntity = await _dbContext.CompanyRecommendations.FirstOrDefaultAsync();

        savedEntity.ShouldNotBeNull();
        savedEntity.CompanyId.ShouldBe(expectedEvent.CompanyId);
        savedEntity.CategoryId.ShouldBe((int)expectedEvent.CompanyCategory);
    }
}