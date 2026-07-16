using BranchService.Contracts.Events.Enums;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using QAuditLogService.Contracts;
using QUserService.Contracts;
using QUserService.Contracts.Events.EmployeeEvent;
using RecommendationService.Application.Consumers.EmployeeConsumers;
using RecommendationService.Application.Interfaces;
using RecommendationService.UnitTests.RecommendationService.Application.Tests.Infrastructure;
using Shouldly;

namespace RecommendationService.UnitTests.RecommendationService.Application.Tests.ConsumerTests.
    EmployeeConsumersTests
{
    public class EmployeeCreatedEventConsumerTests
    {
        private readonly Mock<ILogger<EmployeeCreatedEventConsumer>> _mockLogger;
        private readonly Mock<ConsumeContext<EmployeeCreatedEvent>> _mockContext;
        private readonly IRecommendationDbContext _context;
        private readonly EmployeeCreatedEventConsumer _consumer;

        public EmployeeCreatedEventConsumerTests()
        {
            _mockLogger = new Mock<ILogger<EmployeeCreatedEventConsumer>>();
            _mockContext = new Mock<ConsumeContext<EmployeeCreatedEvent>>();
            _context = TestDbContextFactory.Create();
            _consumer = new EmployeeCreatedEventConsumer(_mockLogger.Object, _context);
        }


        [Fact]
        public async Task Consume_Should_Create_Employee_Recommendation_Entity_When_Event_Received()
        {
            //Arrange
            var occuredAt = DateTime.UtcNow;


            var expectedEvent = new EmployeeCreatedEvent()
            {
                EmployeeId = 1,
                CompanyCategory = CompanyCategory.Healthcare,
                CompanyId = 1,
                BranchId = 1,
                ServiceId = 1,
                FirstName = "Test First Name",
                LastName = "Test Last Name",
                PhoneNumber = "+992922223242",
                Position = "Test Position",
                OccurredAt = occuredAt,
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
            var savedEntity = await _context.EmployeeRecommendations.FirstOrDefaultAsync();

            savedEntity.ShouldNotBeNull();
            savedEntity.EmployeeId.ShouldBe(expectedEvent.EmployeeId);
            savedEntity.CompanyId.ShouldBe(expectedEvent.CompanyId);
           
        }
    }
}