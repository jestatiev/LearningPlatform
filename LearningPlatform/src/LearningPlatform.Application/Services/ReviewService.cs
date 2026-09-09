using LearningPlatform.Application.DTOs;
using LearningPlatform.Application.Exceptions;
using LearningPlatform.Application.Interfaces;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Services;

public class ReviewService : IReviewService
{
    private readonly IUnitOfWork _uow;

    public ReviewService(IUnitOfWork uow) => _uow = uow;

    public async Task<List<ReviewDto>> GetByCourseAsync(int courseId)
    {
        var reviews = await _uow.Reviews.Query()
            .Include(r => r.User)
            .Where(r => r.CourseId == courseId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return reviews.Select(r => new ReviewDto(r.Id, r.Rating, r.Comment, r.CreatedAt, r.User.Username, r.CourseId)).ToList();
    }

    public async Task<ReviewDto> CreateAsync(int userId, CreateReviewDto dto)
    {
        _ = await _uow.Courses.GetByIdAsync(dto.CourseId) ?? throw new NotFoundException(nameof(Course), dto.CourseId);

        var isEnrolled = (await _uow.Enrollments.FindAsync(e => e.UserId == userId && e.CourseId == dto.CourseId)).Any();
        if (!isEnrolled) throw new ForbiddenException("Recenziju možete ostaviti samo za tečajeve na koje ste prijavljeni.");

        var alreadyReviewed = (await _uow.Reviews.FindAsync(r => r.UserId == userId && r.CourseId == dto.CourseId)).Any();
        if (alreadyReviewed) throw new ConflictException("Već ste ostavili recenziju za ovaj tečaj.");

        if (dto.Rating is < 1 or > 5) throw new BadRequestException("Ocjena mora biti između 1 i 5.");

        var review = new Review { UserId = userId, CourseId = dto.CourseId, Rating = dto.Rating, Comment = dto.Comment };
        await _uow.Reviews.AddAsync(review);
        await _uow.SaveChangesAsync();

        var user = await _uow.Users.GetByIdAsync(userId);
        return new ReviewDto(review.Id, review.Rating, review.Comment, review.CreatedAt, user?.Username ?? string.Empty, review.CourseId);
    }

    public async Task UpdateAsync(int userId, int reviewId, UpdateReviewDto dto)
    {
        var review = await _uow.Reviews.GetByIdAsync(reviewId) ?? throw new NotFoundException(nameof(Review), reviewId);
        if (review.UserId != userId) throw new ForbiddenException("Ova recenzija ne pripada vama.");

        if (dto.Rating is < 1 or > 5) throw new BadRequestException("Ocjena mora biti između 1 i 5.");

        review.Rating = dto.Rating;
        review.Comment = dto.Comment;
        _uow.Reviews.Update(review);
        await _uow.SaveChangesAsync();
    }

    public async Task DeleteAsync(int userId, bool isAdmin, int reviewId)
    {
        var review = await _uow.Reviews.GetByIdAsync(reviewId) ?? throw new NotFoundException(nameof(Review), reviewId);
        if (!isAdmin && review.UserId != userId) throw new ForbiddenException("Ova recenzija ne pripada vama.");

        _uow.Reviews.Remove(review);
        await _uow.SaveChangesAsync();
    }
}
