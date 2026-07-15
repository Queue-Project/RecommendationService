using MessagePack;
using RecommendationService.Contracts.Requests;
using Shouldly;

namespace RecommendationService.UnitTests.RecommendationService.Contracts.Tests.RequestTests;

public class GetRecommendedEmployeesRequestTest
{
    [Fact]
    public void RecommendationRequest_ShouldSerializeAndDeserializeCorrectly()
    {
        var originalRequest = new GetRecommendedEmployeesRequest()
        {
            CompanyId = 1,
            BranchId = 1,
            ServiceId = 1,
            PageNumber = 1,
            PageSize = 10
        };

        var bytes = MessagePackSerializer.Serialize(originalRequest);
        var deserializedRequest = MessagePackSerializer.Deserialize<GetRecommendedEmployeesRequest>(bytes);

        deserializedRequest.CompanyId.ShouldBe(originalRequest.CompanyId);
        deserializedRequest.BranchId.ShouldBe(originalRequest.BranchId);
        deserializedRequest.ServiceId.ShouldBe(originalRequest.ServiceId);
        deserializedRequest.PageNumber.ShouldBe(originalRequest.PageNumber);
        deserializedRequest.PageSize.ShouldBe(originalRequest.PageSize);
    }
}