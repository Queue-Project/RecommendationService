using Microsoft.Extensions.Logging;
using Moq;
using RecommendationService.Application.Interfaces;
using RecommendationService.Contracts.Requests;
using RecommendationService.UnitTests.RecommendationService.Application.Tests.Infrastructure;
using Shouldly;

namespace RecommendationService.UnitTests.RecommendationService.Application.Tests.ServiceTests.RecommendationServiceTests;

public class GetRecommendedBranchesTests
{
    private readonly Mock<ILogger<global::RecommendationService.Application.Services.RecommendationService>> _mockLogger;
    private readonly IRecommendationDbContext _context;
    private readonly global::RecommendationService.Application.Services.RecommendationService _service;
    public GetRecommendedBranchesTests()
    {
        _mockLogger = new Mock<ILogger<global::RecommendationService.Application.Services.RecommendationService>>();
        _context = TestDbContextFactory.Create();
        _service = new global::RecommendationService.Application.Services.RecommendationService(_mockLogger.Object,
            _context);
    }

    [Fact]
    public async Task Service_Should_Return_Recommended_Branches_When_Branches_Exists()
    {
        //Arrange
        var branch = TestDataSeeder.CreateBranch();
        await _context.BranchRecommendations.AddAsync(branch, CancellationToken.None);
        await _context.SaveChangesAsync(CancellationToken.None);

        var request = new GetRecommendedBranchesRequest
        {
            CompanyId = 1,
            PageNumber = 1,
            PageSize = 10
        };
        
        
        //Act

        var result = await _service.GetRecommendedBranches(request);

        //Assert

        result.ShouldNotBeNull();
        result.Items.ShouldNotBeEmpty();
        result.PageNumber.ShouldBe(result.PageNumber);
        result.PageSize.ShouldBe(result.PageSize);
        result.HasNextPage.ShouldBe(false);
        result.HasPreviousPage.ShouldBe(false);
        result.TotalCount.ShouldBe(1);
        result.TotalPages.ShouldBe(1);


    }

}