using MessagePack;
using RecommendationService.Contracts.Requests;
using Shouldly;

namespace RecommendationService.UnitTests.RecommendationService.Contracts.Tests.RequestTests;

public class GetRecommendedCompaniesRequestTest
{
    [Fact]
    public void RecommendationRequest_ShouldSerializeAndDeserializeCorrectly()
    {
        var originalRequest = new GetRecommendedCompaniesRequest()
        {
            CategoryId = 1,
            PageNumber = 1,
            PageSize = 10
        };

        var bytes = MessagePackSerializer.Serialize(originalRequest);
        var deserializedRequest = MessagePackSerializer.Deserialize<GetRecommendedCompaniesRequest>(bytes);

        deserializedRequest.CategoryId.ShouldBe(originalRequest.CategoryId);
        deserializedRequest.PageNumber.ShouldBe(originalRequest.PageNumber);
        deserializedRequest.PageSize.ShouldBe(originalRequest.PageSize);
    }
}