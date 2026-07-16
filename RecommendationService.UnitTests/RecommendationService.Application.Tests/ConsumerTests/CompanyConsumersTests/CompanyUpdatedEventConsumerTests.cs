using System.Net;
using BranchService.Contracts.Events;
using BranchService.Contracts.Events.CompanyEvents;
using BranchService.Contracts.Events.Enums;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using QAuditLogService.Contracts;
using RecommendationService.Application.Consumers.CompanyConsumers;
using RecommendationService.Application.Exceptions;
using RecommendationService.Application.Interfaces;
using RecommendationService.UnitTests.RecommendationService.Application.Tests.Infrastructure;
using Shouldly;

namespace RecommendationService.UnitTests.RecommendationService.Application.Tests.ConsumerTests.CompanyConsumersTests
{
    public class CompanyUpdatedEventConsumerTests
    {
        private readonly Mock<ILogger<CompanyUpdatedEventConsumer>> _mockLogger;
        private readonly Mock<ConsumeContext<CompanyUpdatedEvent>> _mockContext;
        private readonly IRecommendationDbContext _context;
        private readonly CompanyUpdatedEventConsumer _consumer;

        public CompanyUpdatedEventConsumerTests()
        {
            _mockLogger = new Mock<ILogger<CompanyUpdatedEventConsumer>>();
            _mockContext = new Mock<ConsumeContext<CompanyUpdatedEvent>>();
            _context = TestDbContextFactory.Create();
            _consumer = new CompanyUpdatedEventConsumer(_mockLogger.Object, _context);
        }


        [Fact]
        public async Task Consume_Should_Updated_Company_Recommendation_Entity_When_Event_Received()
        {
            //Arrange
            var company = TestDataSeeder.CreateCompany();
            await _context.CompanyRecommendations.AddAsync(company, CancellationToken.None);
            await _context.SaveChangesAsync(CancellationToken.None);

            var occuredAt = DateTime.UtcNow;


            var expectedEvent = new CompanyUpdatedEvent()
            {
                CompanyId = 1,
                CompanyCategory = CompanyCategory.Healthcare,
                CompanyName = "Update Company Name",
                EmailAddress = "update@gmail.com",
                Address = "Update Address",
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
            var updatedEntity = await _context.CompanyRecommendations.FirstOrDefaultAsync();

            updatedEntity.ShouldNotBeNull();
            updatedEntity.CompanyId.ShouldBe(expectedEvent.CompanyId);
            updatedEntity.CategoryId.ShouldBe((int)expectedEvent.CompanyCategory);
        }

        [Fact]
        public async Task Consume_Should_Throw_Not_Found_When_Event_Received_And_Entity_Not_Found()
        {
            //Arrange

            var occuredAt = DateTime.UtcNow;


            var expectedEvent = new CompanyUpdatedEvent()
            {
                CompanyId = 1,
                CompanyCategory = CompanyCategory.Healthcare,
                CompanyName = "Update Company Name",
                EmailAddress = "update@gmail.com",
                Address = "Update Address",
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


            // Act & Assert
            var exception =
                await Should.ThrowAsync<HttpStatusCodeException>(async () =>
                    await _consumer.Consume(_mockContext.Object)
                );

            exception.StatusCode.ShouldBe(HttpStatusCode.NotFound);
            exception.Message.ShouldBe(
                $"Company with Id {expectedEvent.CompanyId} not found in Company Recommendation Entity");
        }
    }
}