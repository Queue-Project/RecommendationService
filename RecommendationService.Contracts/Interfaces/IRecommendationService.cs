using MagicOnion;
using RecommendationService.Contracts.Requests;
using RecommendationService.Contracts.Responses;

namespace RecommendationService.Contracts.Interfaces;

public interface IRecommendationService : IService<IRecommendationService>
{
    UnaryResult<PagedResponse<RecommendedCompanyResponse>> GetRecommendedCompanies(
        GetRecommendedCompaniesRequest recommendedCompaniesRequest);

    UnaryResult<PagedResponse<RecommendedBranchResponse>> GetRecommendedBranches(
        GetRecommendedBranchesRequest recommendedBranchesRequest);

    UnaryResult<PagedResponse<RecommendedServiceResponse>> GetRecommendedServices(
        GetRecommendedServicesRequest recommendedServicesRequest);

    UnaryResult<PagedResponse<RecommendedEmployeeResponse>> GetRecommendedEmployees(
        GetRecommendedEmployeesRequest recommendedEmployeesRequest);
}