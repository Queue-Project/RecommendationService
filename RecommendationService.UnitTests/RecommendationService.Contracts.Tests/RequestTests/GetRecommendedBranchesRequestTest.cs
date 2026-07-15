using MessagePack;
using RecommendationService.Contracts.Requests;
using Shouldly;

namespace RecommendationService.UnitTests.RecommendationService.Contracts.Tests.RequestTests;

public class GetRecommendedBranchesRequestTest
{
    [Fact]
    public void RecommendationRequest_ShouldSerializeAndDeserializeCorrectly()
    {
        var originalRequest = new GetRecommendedBranchesRequest()
        {
            CompanyId = 1,
            PageNumber = 1,
            PageSize = 10
        };

        var bytes = MessagePackSerializer.Serialize(originalRequest);
        var deserializedRequest = MessagePackSerializer.Deserialize<GetRecommendedBranchesRequest>(bytes);

        deserializedRequest.CompanyId.ShouldBe(originalRequest.CompanyId);
        deserializedRequest.PageNumber.ShouldBe(originalRequest.PageNumber);
        deserializedRequest.PageSize.ShouldBe(originalRequest.PageSize);
    }
}