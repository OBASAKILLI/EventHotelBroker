using EventHotelBroker.Data;
using EventHotelBroker.Models;
using Microsoft.EntityFrameworkCore;

namespace EventHotelBroker.Services;

public class ReviewService : IReviewService
{
    private readonly ApplicationDbContext _context;

    public ReviewService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Review>> GetReviewsAsync(string entityType, int entityId)
    {
        return await _context.Reviews
            .Include(r => r.User)
            .Where(r => r.EntityType == entityType && r.EntityId == entityId && r.IsApproved)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<Review?> GetUserReviewAsync(string entityType, int entityId, string userId)
    {
        return await _context.Reviews
            .FirstOrDefaultAsync(r => r.EntityType == entityType && r.EntityId == entityId && r.UserId == userId);
    }

    public async Task<Review> AddReviewAsync(Review review)
    {
        // Check if user already reviewed this entity
        var existingReview = await GetUserReviewAsync(review.EntityType, review.EntityId, review.UserId);
        if (existingReview != null)
        {
            throw new InvalidOperationException("You have already reviewed this.");
        }

        review.CreatedAt = DateTime.UtcNow;
        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();
        
        return review;
    }

    public async Task<bool> DeleteReviewAsync(int reviewId, string userId)
    {
        var review = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == reviewId);
        if (review == null) return false;

        // Ensure user owns the review (or check admin role if applicable)
        if (review.UserId != userId) return false;

        _context.Reviews.Remove(review);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<(double AverageRating, int TotalCount)> GetRatingSummaryAsync(string entityType, int entityId)
    {
        var reviews = await _context.Reviews
            .Where(r => r.EntityType == entityType && r.EntityId == entityId && r.IsApproved)
            .Select(r => r.Rating)
            .ToListAsync();

        if (!reviews.Any()) return (0, 0);

        return (reviews.Average(), reviews.Count);
    }
}
