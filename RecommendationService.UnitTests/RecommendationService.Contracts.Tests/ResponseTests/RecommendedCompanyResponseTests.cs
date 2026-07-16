using MessagePack;
using RecommendationService.Contracts.Responses;
using Shouldly;

namespace RecommendationService.UnitTests.RecommendationService.Contracts.Tests.ResponseTests;

public class RecommendedCompanyResponseTests
{
    [Fact]
    public void RecommendationResponse_ShouldSerializeAndDeserializeCorrectly()
    {
        var originalRequest = new RecommendedCompanyResponse()
        {
            CategoryId = 1,
            CompanyId = 1,
            RecommendationScore = 60,
            AverageRating = 4.5,
            ReviewCount = 5,
            CompletedQueues = 10,
            ComplaintCount = 0,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        var bytes = MessagePackSerializer.Serialize(originalRequest);
        var deserializedRequest = MessagePackSerializer.Deserialize<RecommendedCompanyResponse>(bytes);

        deserializedRequest.CategoryId.ShouldBe(originalRequest.CategoryId);
        deserializedRequest.CompanyId.ShouldBe(originalRequest.CompanyId);
        deserializedRequest.RecommendationScore.ShouldBe(originalRequest.RecommendationScore);
        deserializedRequest.AverageRating.ShouldBe(originalRequest.AverageRating);
        deserializedRequest.ReviewCount.ShouldBe(originalRequest.ReviewCount);
        deserializedRequest.CompletedQueues.ShouldBe(originalRequest.CompletedQueues);
        deserializedRequest.ComplaintCount.ShouldBe(originalRequest.ComplaintCount);
        deserializedRequest.UpdatedAt.ShouldBe(originalRequest.UpdatedAt);
    }
}