using LearningPlatform.Application.DTOs;
using LearningPlatform.Application.Exceptions;
using LearningPlatform.Application.Interfaces;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Services;

public class EnrollmentService : IEnrollmentService
{
    private readonly IUnitOfWork _uow;

    public EnrollmentService(IUnitOfWork uow) => _uow = uow;

    public async Task<List<EnrollmentDto>> GetMyEnrollmentsAsync(int userId)
    {
        var enrollments = await _uow.Enrollments.Query()
            .Where(e => e.UserId == userId)
            .Select(e => new EnrollmentDto(e.Id, e.CourseId, e.Course.Title, e.EnrolledAt, e.ProgressPercent, e.IsCompleted))
            .ToListAsync();
        return enrollments;
    }

    public async Task<EnrollmentDto> EnrollAsync(int userId, CreateEnrollmentDto dto)
    {
        var course = await _uow.Courses.GetByIdAsync(dto.CourseId) ?? throw new NotFoundException(nameof(Course), dto.CourseId);

        var already = (await _uow.Enrollments.FindAsync(e => e.UserId == userId && e.CourseId == dto.CourseId)).Any();
        if (already) throw new ConflictException("Već ste prijavljeni na ovaj tečaj.");

        var enrollment = new Enrollment { UserId = userId, CourseId = dto.CourseId };
        await _uow.Enrollments.AddAsync(enrollment);
        await _uow.SaveChangesAsync();

        return new EnrollmentDto(enrollment.Id, course.Id, course.Title, enrollment.EnrolledAt, enrollment.ProgressPercent, enrollment.IsCompleted);
    }

    public async Task UpdateProgressAsync(int userId, int enrollmentId, UpdateProgressDto dto)
    {
        var enrollment = await _uow.Enrollments.GetByIdAsync(enrollmentId) ?? throw new NotFoundException(nameof(Enrollment), enrollmentId);
        if (enrollment.UserId != userId) throw new ForbiddenException("Ova prijava na tečaj ne pripada vama.");

        enrollment.ProgressPercent = Math.Clamp(dto.ProgressPercent, 0, 100);
        enrollment.IsCompleted = dto.IsCompleted;
        _uow.Enrollments.Update(enrollment);
        await _uow.SaveChangesAsync();
    }

    public async Task UnenrollAsync(int userId, int enrollmentId)
    {
        var enrollment = await _uow.Enrollments.GetByIdAsync(enrollmentId) ?? throw new NotFoundException(nameof(Enrollment), enrollmentId);
        if (enrollment.UserId != userId) throw new ForbiddenException("Ova prijava na tečaj ne pripada vama.");

        _uow.Enrollments.Remove(enrollment);
        await _uow.SaveChangesAsync();
    }

    public async Task<List<EnrollmentDto>> GetByCourseAsync(int courseId)
    {
        var enrollments = await _uow.Enrollments.Query()
            .Where(e => e.CourseId == courseId)
            .Select(e => new EnrollmentDto(e.Id, e.CourseId, e.Course.Title, e.EnrolledAt, e.ProgressPercent, e.IsCompleted))
            .ToListAsync();
        return enrollments;
    }
}
