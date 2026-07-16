using RecommendationService.Domain.Models;

namespace RecommendationService.Application.Interfaces;

public interface IRecommendationScoreService
{
    double CalculateRecommendationScore(BaseRecommendationEntity recommendation);
}