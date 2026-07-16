using System.Net;
using BranchService.Contracts.Events;
using BranchService.Contracts.Events.BranchEvents;
using BranchService.Contracts.Events.Enums;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using QAuditLogService.Contracts;
using RecommendationService.Application.Consumers.BranchConsumers;
using RecommendationService.Application.Exceptions;
using RecommendationService.Application.Interfaces;
using RecommendationService.UnitTests.RecommendationService.Application.Tests.Infrastructure;
using Shouldly;

namespace RecommendationService.UnitTests.RecommendationService.Application.Tests.ConsumerTests.BranchConsumersTests;

public class BranchDeletedEventConsumerTests
{
    private readonly Mock<ILogger<BranchDeletedEventConsumer>> _mockLogger;
    private readonly Mock<ConsumeContext<BranchDeletedEvent>> _mockContext;
    private readonly IRecommendationDbContext _context;
    private readonly BranchDeletedEventConsumer _consumer;

    public BranchDeletedEventConsumerTests()
    {
        _mockLogger = new Mock<ILogger<BranchDeletedEventConsumer>>();
        _mockContext = new Mock<ConsumeContext<BranchDeletedEvent>>();
        _context = TestDbContextFactory.Create();
        _consumer = new BranchDeletedEventConsumer(_mockLogger.Object, _context);
    }
    
    [Fact]
    public async Task Consume_Should_Delete_Branch_Recommendation_Entity_When_Event_Received()
    {
        //Arrange

        var branch = TestDataSeeder.CreateBranch();
        await _context.BranchRecommendations.AddAsync(branch, CancellationToken.None);
        await _context.SaveChangesAsync(CancellationToken.None);
        
        var occuredAt = DateTime.UtcNow;


        var expectedEvent = new BranchDeletedEvent()
        {
            CompanyId = 1,
            BranchId = 1,
            BranchName = "Updated Branch Name",
            EmailAddress = "test@gmail.com",
            Address = "Updated Address",
            City = "Updated City",
            IsActive = true,
            PhoneNumber = "+992923324211",
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
        
        var deletedEntity = await _context.BranchRecommendations
            .FirstOrDefaultAsync(s => s.BranchId == expectedEvent.BranchId);

        deletedEntity.ShouldBeNull(); 

        var remainingEntities = await _context.BranchRecommendations.ToListAsync();
        remainingEntities.ShouldBeEmpty();
        
    }
    
    [Fact]
    public async Task Consume_Should_Throw_Not_Found_When_Event_Received_And_Entity_Not_Found()
    {
        //Arrange
        
        var occuredAt = DateTime.UtcNow;


        var expectedEvent = new BranchDeletedEvent()
        {
            CompanyId = 1,
            BranchId = 1,
            CompanyCategory = CompanyCategory.Healthcare,
            BranchName = "Updated Branch Name",
            EmailAddress = "test@gmail.com",
            Address = "Updated Address",
            City = "Updated City",
            IsActive = true,
            PhoneNumber = "+992923324211",
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

        // Act & Assert
        var exception =
            await Should.ThrowAsync<HttpStatusCodeException>(async () =>
                await _consumer.Consume(_mockContext.Object)
            );

        exception.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        exception.Message.ShouldBe(
            $"Branch with Id {expectedEvent.BranchId} not found in Branch Recommendation Entity");
      
    }
}