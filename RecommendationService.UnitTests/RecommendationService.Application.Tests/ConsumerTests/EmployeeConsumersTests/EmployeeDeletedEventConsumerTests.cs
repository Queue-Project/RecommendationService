using System.Net;
using BranchService.Contracts.Events.Enums;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using QAuditLogService.Contracts;
using QUserService.Contracts;
using QUserService.Contracts.Events.EmployeeEvent;
using RecommendationService.Application.Consumers.EmployeeConsumers;
using RecommendationService.Application.Exceptions;
using RecommendationService.Application.Interfaces;
using RecommendationService.UnitTests.RecommendationService.Application.Tests.Infrastructure;
using Shouldly;

namespace RecommendationService.UnitTests.RecommendationService.Application.Tests.ConsumerTests.EmployeeConsumersTests
{
    public class EmployeeDeletedEventConsumerTests
    {
        private readonly Mock<ILogger<EmployeeDeletedEventConsumer>> _mockLogger;
        private readonly Mock<ConsumeContext<EmployeeDeletedEvent>> _mockContext;
        private readonly IRecommendationDbContext _context;
        private readonly EmployeeDeletedEventConsumer _consumer;

        public EmployeeDeletedEventConsumerTests()
        {
            _mockLogger = new Mock<ILogger<EmployeeDeletedEventConsumer>>();
            _mockContext = new Mock<ConsumeContext<EmployeeDeletedEvent>>();
            _context = TestDbContextFactory.Create();
            _consumer = new EmployeeDeletedEventConsumer(_mockLogger.Object, _context);
        }


        [Fact]
        public async Task Consume_Should_Delete_Employee_Recommendation_Entity_When_Event_Received()
        {
            //Arrange
            var employee = TestDataSeeder.CreateEmployee();
            await _context.EmployeeRecommendations.AddAsync(employee, CancellationToken.None);
            await _context.SaveChangesAsync(CancellationToken.None);
            
            var occuredAt = DateTime.UtcNow;


            var expectedEvent = new EmployeeDeletedEvent()
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
            var deletedEntity = await _context.EmployeeRecommendations
                .FirstOrDefaultAsync(s => s.EmployeeId == expectedEvent.EmployeeId);

            deletedEntity.ShouldBeNull(); 

            var remainingEntities = await _context.EmployeeRecommendations.ToListAsync();
            remainingEntities.ShouldBeEmpty();

        }
        
        [Fact]
        public async Task Consume_Should_Throw_Not_Found_When_Event_Received_And_Entity_Not_Found()
        {
            //Arrange
        
            var occuredAt = DateTime.UtcNow;


            var expectedEvent = new EmployeeDeletedEvent()
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

            // Act & Assert
            var exception =
                await Should.ThrowAsync<HttpStatusCodeException>(async () =>
                    await _consumer.Consume(_mockContext.Object)
                );

            exception.StatusCode.ShouldBe(HttpStatusCode.NotFound);
            exception.Message.ShouldBe(
                $"Employee with Id {expectedEvent.EmployeeId} not found in Employee Recommendation Entity");
        }
    }
}