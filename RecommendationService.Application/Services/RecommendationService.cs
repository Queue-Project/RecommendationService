using MagicOnion;
using MagicOnion.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RecommendationService.Application.Interfaces;
using RecommendationService.Contracts.Interfaces;
using RecommendationService.Contracts.Requests;
using RecommendationService.Contracts.Responses;

namespace RecommendationService.Application.Services;

public class RecommendationService : ServiceBase<IRecommendationService>, IRecommendationService
{
    private readonly ILogger<RecommendationService> _logger;
    private readonly IRecommendationDbContext _dbContext;

    public RecommendationService(ILogger<RecommendationService> logger, IRecommendationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }


    public async UnaryResult<PagedResponse<RecommendedCompanyResponse>> GetRecommendedCompanies(
        GetRecommendedCompaniesRequest recommendedCompaniesRequest)
    {
        _logger.LogInformation("Fetching recommended companies for CategoryId: {CategoryId}",
            recommendedCompaniesRequest.CategoryId);

        var query = _dbContext.CompanyRecommendations
            .AsNoTracking()
            .Where(s => s.CategoryId == recommendedCompaniesRequest.CategoryId);


        var totalCount = await query.CountAsync();

        var companies = await query
            .OrderByDescending(s => s.RecommendationScore)
            .ThenByDescending(s => s.AverageRating)
            .Skip((recommendedCompaniesRequest.PageNumber - 1) * recommendedCompaniesRequest.PageSize)
            .Take(recommendedCompaniesRequest.PageSize)
            .ToListAsync();

        var items = companies.Select(s => new RecommendedCompanyResponse
        {
            CategoryId = s.CategoryId,
            CompanyId = s.CompanyId,
            RecommendationScore = s.RecommendationScore,
            CompletedQueues = s.CompletedQueues,
            ReviewCount = s.ReviewCount,
            AverageRating = s.AverageRating,
            ComplaintCount = s.ComplaintCount,
            UpdatedAt = s.UpdatedAt
        }).ToList();

        return new PagedResponse<RecommendedCompanyResponse>
        {
            Items = items,
            PageNumber = recommendedCompaniesRequest.PageNumber,
            PageSize = recommendedCompaniesRequest.PageSize,
            TotalCount = totalCount
        };
    }

    public async UnaryResult<PagedResponse<RecommendedBranchResponse>> GetRecommendedBranches(
        GetRecommendedBranchesRequest recommendedBranchesRequest)
    {
        _logger.LogInformation("Fetching recommended branches for CompanyId: {CompanyId}",
            recommendedBranchesRequest.CompanyId);

        var query = _dbContext.BranchRecommendations
            .AsNoTracking()
            .Where(s => s.CompanyId == recommendedBranchesRequest.CompanyId);


        var totalCount = await query.CountAsync();
        var branches = await query
            .OrderByDescending(s => s.RecommendationScore)
            .ThenByDescending(s => s.AverageRating)
            .Skip((recommendedBranchesRequest.PageNumber - 1) * recommendedBranchesRequest.PageSize)
            .Take(recommendedBranchesRequest.PageSize)
            .ToListAsync();

        var items = branches.Select(s => new RecommendedBranchResponse()
        {
            BranchId = s.BranchId,
            CompanyId = s.CompanyId,
            CategoryId = s.CategoryId,
            RecommendationScore = s.RecommendationScore,
            CompletedQueues = s.CompletedQueues,
            ReviewCount = s.ReviewCount,
            AverageRating = s.AverageRating,
            ComplaintCount = s.ComplaintCount,
            UpdatedAt = s.UpdatedAt
        }).ToList();

        return new PagedResponse<RecommendedBranchResponse>
        {
            Items = items,
            PageNumber = recommendedBranchesRequest.PageNumber,
            PageSize = recommendedBranchesRequest.PageSize,
            TotalCount = totalCount
        };
    }

    public async UnaryResult<PagedResponse<RecommendedServiceResponse>> GetRecommendedServices(
        GetRecommendedServicesRequest recommendedServicesRequest)
    {
        _logger.LogInformation("Fetching recommended services for CompanyId: {CompanyId} and BranchId: {BranchId}",
            recommendedServicesRequest.CompanyId, recommendedServicesRequest.BranchId);

        var query = _dbContext.ServiceRecommendations
            .AsNoTracking()
            .Where(s => s.CompanyId == recommendedServicesRequest.CompanyId &&
                        s.BranchId == recommendedServicesRequest.BranchId);

        var totalCount = await query.CountAsync();
        var services = await query
            .OrderByDescending(s => s.RecommendationScore)
            .ThenByDescending(s => s.AverageRating)
            .Skip((recommendedServicesRequest.PageNumber - 1) * recommendedServicesRequest.PageSize)
            .Take(recommendedServicesRequest.PageSize)
            .ToListAsync();

        var items = services.Select(s => new RecommendedServiceResponse
        {
            ServiceId = s.ServiceId,
            BranchId = s.BranchId,
            CompanyId = s.CompanyId,
            CategoryId = s.CategoryId,
            RecommendationScore = s.RecommendationScore,
            CompletedQueues = s.CompletedQueues,
            ReviewCount = s.ReviewCount,
            AverageRating = s.AverageRating,
            ComplaintCount = s.ComplaintCount,
            UpdatedAt = s.UpdatedAt
        }).ToList();

        return new PagedResponse<RecommendedServiceResponse>
        {
            Items = items,
            PageNumber = recommendedServicesRequest.PageNumber,
            PageSize = recommendedServicesRequest.PageSize,
            TotalCount = totalCount
        };
    }

    public async UnaryResult<PagedResponse<RecommendedEmployeeResponse>> GetRecommendedEmployees(
        GetRecommendedEmployeesRequest recommendedEmployeesRequest)
    {
        _logger.LogInformation(
            "Fetching recommended employees for CompanyId: {CompanyId}  BranchId: {BranchId} and ServiceId: {ServiceId}",
            recommendedEmployeesRequest.CompanyId, recommendedEmployeesRequest.BranchId,
            recommendedEmployeesRequest.ServiceId);

        var query = _dbContext.EmployeeRecommendations
            .AsNoTracking()
            .Where(s => s.CompanyId == recommendedEmployeesRequest.CompanyId &&
                        s.BranchId == recommendedEmployeesRequest.BranchId &&
                        s.ServiceId == recommendedEmployeesRequest.ServiceId );

        var totalCount = await query.CountAsync();
        var employees = await query
            .OrderByDescending(s => s.RecommendationScore)
            .ThenByDescending(s => s.AverageRating)
            .Skip((recommendedEmployeesRequest.PageNumber - 1) * recommendedEmployeesRequest.PageSize)
            .Take(recommendedEmployeesRequest.PageSize)
            .ToListAsync();

        var items = employees.Select(s => new RecommendedEmployeeResponse()
        {
            EmployeeId = s.EmployeeId,
            ServiceId = s.ServiceId,
            BranchId = s.BranchId,
            CompanyId = s.CompanyId,
            CategoryId = s.CategoryId,
            RecommendationScore = s.RecommendationScore,
            CompletedQueues = s.CompletedQueues,
            ReviewCount = s.ReviewCount,
            AverageRating = s.AverageRating,
            ComplaintCount = s.ComplaintCount,
            UpdatedAt = s.UpdatedAt
        }).ToList();

        return new PagedResponse<RecommendedEmployeeResponse>
        {
            Items = items,
            PageNumber = recommendedEmployeesRequest.PageNumber,
            PageSize = recommendedEmployeesRequest.PageSize,
            TotalCount = totalCount
        };
    }
}