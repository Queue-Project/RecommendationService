using BranchService.Contracts.Events;
using BranchService.Contracts.Events.BranchEvents;
using BranchService.Contracts.Events.Enums;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using QAuditLogService.Contracts;
using RecommendationService.Application.Consumers.BranchConsumers;
using RecommendationService.Application.Interfaces;
using RecommendationService.UnitTests.RecommendationService.Application.Tests.Infrastructure;
using Shouldly;

namespace RecommendationService.UnitTests.RecommendationService.Application.Tests.ConsumerTests.BranchConsumersTests
{
    public class BranchCreatedEventConsumerTests
    {
        private readonly Mock<ILogger<BranchCreatedEventConsumer>> _mockLogger;
        private readonly Mock<ConsumeContext<BranchCreatedEvent>> _mockContext;
        private readonly IRecommendationDbContext _context;
        private readonly BranchCreatedEventConsumer _consumer;

        public BranchCreatedEventConsumerTests()
        {
            _mockLogger = new Mock<ILogger<BranchCreatedEventConsumer>>();
            _mockContext = new Mock<ConsumeContext<BranchCreatedEvent>>();
            _context = TestDbContextFactory.Create();
            _consumer = new BranchCreatedEventConsumer(_mockLogger.Object, _context);
        }


        [Fact]
        public async Task Consume_Should_Create_Branch_Recommendation_Entity_When_Event_Received()
        {
            //Arrange
            var occuredAt = DateTime.UtcNow;


            var expectedEvent = new BranchCreatedEvent()
            {
                CompanyId = 1,
                CompanyCategory = CompanyCategory.Healthcare,
                BranchId = 1,
                BranchName = "Test Branch Name",
                EmailAddress = "test@gmail.com",
                Address = "Test Address",
                City = "Test City",
                IsActive = true,
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
            var savedEntity = await _context.BranchRecommendations.FirstOrDefaultAsync();

            savedEntity.ShouldNotBeNull();
            savedEntity.BranchId.ShouldBe(expectedEvent.BranchId);
            savedEntity.CategoryId.ShouldBe((int)expectedEvent.CompanyCategory);
        }
    }
}