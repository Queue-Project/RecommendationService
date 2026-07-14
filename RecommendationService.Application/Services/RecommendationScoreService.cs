using RecommendationService.Application.Interfaces;
using RecommendationService.Domain.Models;

namespace RecommendationService.Application.Services;

public class RecommendationScoreService : IRecommendationScoreService
{
    private const double MaxRating = 5.0;
    private const int ReviewsNeededForFullTrust = 50;
    private const int QueuesNeededForFullExperience = 1000;

    public double CalculateRecommendationScore(BaseRecommendationEntity recommendation)
    {

        double ratingScore = (recommendation.AverageRating / MaxRating) * 100;

      
        double ratingConfidence = 0.5;
        
        if (recommendation.ReviewCount < ReviewsNeededForFullTrust)
        {
           
            ratingConfidence = 0.5 + (0.5 * ((double)recommendation.ReviewCount / ReviewsNeededForFullTrust));
        }
        else
        {
            ratingConfidence = 1.0;
        }

       
        double trustedRatingScore = ratingScore * ratingConfidence;

        int totalQueues = recommendation.CompletedQueues + recommendation.CancelledQueues;
        double completionRate = 0;

        if (totalQueues == 0)
        {
            completionRate = 50;
        }
        else
        {
            completionRate = ((double)recommendation.CompletedQueues / totalQueues) * 100;
        }


        double experienceScore = 0;
        
        if (recommendation.CompletedQueues >= QueuesNeededForFullExperience)
        {
            experienceScore = 100;
        }
        else
        {
            experienceScore = ((double)recommendation.CompletedQueues / QueuesNeededForFullExperience) * 100;
        }

        double complaintPenalty = 0;

        if (recommendation.CompletedQueues > 0)
        {
            complaintPenalty = ((double)recommendation.ComplaintCount / recommendation.CompletedQueues) * 100;
        }

        double score =
            (trustedRatingScore * 0.45) +
            (completionRate * 0.30) +
            (experienceScore * 0.15) -
            (complaintPenalty * 0.10);    

        
        return Math.Clamp(score, 0, 100);
        
    }
}