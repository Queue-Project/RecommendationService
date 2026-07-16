using BranchService.Contracts.Events;
using BranchService.Contracts.Events.CompanyServiceEvents;
using BranchService.Contracts.Events.Enums;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using QAuditLogService.Contracts;
using RecommendationService.Application.Consumers.CompanyServiceConsumers;
using RecommendationService.Application.Interfaces;
using RecommendationService.UnitTests.RecommendationService.Application.Tests.Infrastructure;
using Shouldly;

namespace RecommendationService.UnitTests.RecommendationService.Application.Tests.ConsumerTests.
    CompanyServiceConsumersTests
{
    public class CompanyServiceCreatedEventConsumerTests
    {
        private readonly Mock<ILogger<CompanyServiceCreatedEventConsumer>> _mockLogger;
        private readonly Mock<ConsumeContext<CompanyServiceCreatedEvent>> _mockContext;
        private readonly IRecommendationDbContext _context;
        private readonly CompanyServiceCreatedEventConsumer _consumer;

        public CompanyServiceCreatedEventConsumerTests()
        {
            _mockLogger = new Mock<ILogger<CompanyServiceCreatedEventConsumer>>();
            _mockContext = new Mock<ConsumeContext<CompanyServiceCreatedEvent>>();
            _context = TestDbContextFactory.Create();
            _consumer = new CompanyServiceCreatedEventConsumer(_mockLogger.Object, _context);
        }


        [Fact]
        public async Task Consume_Should_Create_Service_Recommendation_Entity_When_Event_Received()
        {
            //Arrange
            var occuredAt = DateTime.UtcNow;


            var expectedEvent = new CompanyServiceCreatedEvent()
            {
                CompanyId = 1,
                CompanyServiceId = 1,
                BranchId = 1,
                CompanyCategory = CompanyCategory.Healthcare,
                ServiceName = "Test Service Name",
                ServiceDescription = "Test Description",
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
            var savedEntity = await _context.ServiceRecommendations.FirstOrDefaultAsync();

            savedEntity.ShouldNotBeNull();
            savedEntity.ServiceId.ShouldBe(expectedEvent.CompanyServiceId);
            savedEntity.CompanyId.ShouldBe(expectedEvent.CompanyId);
            savedEntity.BranchId.ShouldBe(expectedEvent.BranchId);
            savedEntity.CategoryId.ShouldBe((int)expectedEvent.CompanyCategory);
        }
    }
}