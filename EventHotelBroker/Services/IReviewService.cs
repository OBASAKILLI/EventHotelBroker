using EventHotelBroker.Models;

namespace EventHotelBroker.Services;

public interface IReviewService
{
    Task<List<Review>> GetReviewsAsync(string entityType, int entityId);
    Task<Review?> GetUserReviewAsync(string entityType, int entityId, string userId);
    Task<Review> AddReviewAsync(Review review);
    Task<bool> DeleteReviewAsync(int reviewId, string userId);
    Task<(double AverageRating, int TotalCount)> GetRatingSummaryAsync(string entityType, int entityId);
}
