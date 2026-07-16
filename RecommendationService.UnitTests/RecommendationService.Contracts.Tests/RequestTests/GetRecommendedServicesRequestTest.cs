using MessagePack;
using RecommendationService.Contracts.Requests;
using Shouldly;

namespace RecommendationService.UnitTests.RecommendationService.Contracts.Tests.RequestTests;

public class GetRecommendedServicesRequestTest
{
    [Fact]
    public void RecommendationRequest_ShouldSerializeAndDeserializeCorrectly()
    {
        var originalRequest = new GetRecommendedServicesRequest()
        {
            CompanyId = 1,
            BranchId = 1,
            PageNumber = 1,
            PageSize = 10
        };

        var bytes = MessagePackSerializer.Serialize(originalRequest);
        var deserializedRequest = MessagePackSerializer.Deserialize<GetRecommendedServicesRequest>(bytes);

        deserializedRequest.CompanyId.ShouldBe(originalRequest.CompanyId);
        deserializedRequest.BranchId.ShouldBe(originalRequest.BranchId);
        deserializedRequest.PageNumber.ShouldBe(originalRequest.PageNumber);
        deserializedRequest.PageSize.ShouldBe(originalRequest.PageSize);
    }
}