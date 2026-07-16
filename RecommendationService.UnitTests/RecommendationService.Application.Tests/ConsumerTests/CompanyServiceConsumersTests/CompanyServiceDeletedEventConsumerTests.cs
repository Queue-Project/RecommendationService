using System.Net;
using BranchService.Contracts.Events;
using BranchService.Contracts.Events.CompanyServiceEvents;
using BranchService.Contracts.Events.Enums;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using QAuditLogService.Contracts;
using RecommendationService.Application.Consumers.CompanyServiceConsumers;
using RecommendationService.Application.Exceptions;
using RecommendationService.Application.Interfaces;
using RecommendationService.UnitTests.RecommendationService.Application.Tests.Infrastructure;
using Shouldly;

namespace RecommendationService.UnitTests.RecommendationService.Application.Tests.ConsumerTests.CompanyServiceConsumersTests
{
    public class CompanyServiceDeletedEventConsumerTests
    {
        private readonly Mock<ILogger<CompanyServiceDeletedEventConsumer>> _mockLogger;
        private readonly Mock<ConsumeContext<CompanyServiceDeletedEvent>> _mockContext;
        private readonly IRecommendationDbContext _context;
        private readonly CompanyServiceDeletedEventConsumer _consumer;

        public CompanyServiceDeletedEventConsumerTests()
        {
            _mockLogger = new Mock<ILogger<CompanyServiceDeletedEventConsumer>>();
            _mockContext = new Mock<ConsumeContext<CompanyServiceDeletedEvent>>();
            _context = TestDbContextFactory.Create();
            _consumer = new CompanyServiceDeletedEventConsumer(_mockLogger.Object, _context);
        }


        [Fact]
        public async Task Consume_Should_Delete_Service_Recommendation_Entity_When_Event_Received()
        {
            //Arrange
            var service = TestDataSeeder.CreateCompanyService();
            await _context.ServiceRecommendations.AddAsync(service, CancellationToken.None);
            await _context.SaveChangesAsync(CancellationToken.None);
            
            var occuredAt = DateTime.UtcNow;


            var expectedEvent = new CompanyServiceDeletedEvent()
            {
                CompanyId = 1,
                CompanyServiceId = 1,
                BranchId = 1,
                CompanyCategory = CompanyCategory.Healthcare,
                ServiceName = "Update Company Service Name",
                ServiceDescription = "Update Description",
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
            var deletedEntity = await _context.ServiceRecommendations
                .FirstOrDefaultAsync(s => s.ServiceId == expectedEvent.CompanyServiceId);

            deletedEntity.ShouldBeNull(); 

            var remainingEntity = await _context.ServiceRecommendations.ToListAsync();
            remainingEntity.ShouldBeEmpty();

        }
        
        [Fact]
        public async Task Consume_Should_Throw_Not_Found_When_Event_Received_And_Entity_Not_Found()
        {
            //Arrange
        
            var occuredAt = DateTime.UtcNow;


            var expectedEvent = new CompanyServiceDeletedEvent()
            {
                CompanyId = 1,
                CompanyServiceId = 1,
                BranchId = 1,
                CompanyCategory = CompanyCategory.Healthcare,
                ServiceName = "Update Company Service Name",
                ServiceDescription = "Update Description",
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
                $"CompanyService with Id {expectedEvent.CompanyServiceId} not found in CompanyService Recommendation Entity");
        }
    }
}