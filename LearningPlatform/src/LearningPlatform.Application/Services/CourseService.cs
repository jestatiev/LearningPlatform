using LearningPlatform.Application.DTOs;
using LearningPlatform.Application.Exceptions;
using LearningPlatform.Application.Interfaces;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Services;

public class CourseService : ICourseService
{
    private readonly IUnitOfWork _uow;

    public CourseService(IUnitOfWork uow) => _uow = uow;

    public async Task<List<CourseListItemDto>> GetAllAsync(int? categoryId)
    {
        var courses = await _uow.Courses.Query()
            .Include(c => c.Category)
            .Include(c => c.Reviews)
            .Where(c => categoryId == null || c.CategoryId == categoryId)
            .ToListAsync();

        return courses.Select(c => new CourseListItemDto(
            c.Id, c.Title, c.InstructorName, c.Price, c.Category.Name,
            c.Reviews.Count > 0 ? Math.Round(c.Reviews.Average(r => r.Rating), 2) : 0)).ToList();
    }

    public async Task<CourseDetailsDto> GetByIdAsync(int id)
    {
        var course = await _uow.Courses.Query()
            .Include(c => c.Category)
            .Include(c => c.Lessons)
            .Include(c => c.Reviews).ThenInclude(r => r.User)
            .Include(c => c.Enrollments)
            .FirstOrDefaultAsync(c => c.Id == id)
            ?? throw new NotFoundException(nameof(Course), id);

        return MapToDetails(course);
    }

    public async Task<CourseDetailsDto> CreateAsync(CreateCourseDto dto)
    {
        var categoryExists = await _uow.Categories.GetByIdAsync(dto.CategoryId)
            ?? throw new BadRequestException("Odabrana kategorija ne postoji.");

        var course = new Course
        {
            Title = dto.Title,
            Description = dto.Description,
            InstructorName = dto.InstructorName,
            Price = dto.Price,
            CategoryId = dto.CategoryId,
        };
        await _uow.Courses.AddAsync(course);
        await _uow.SaveChangesAsync();

        course.Category = categoryExists;
        return MapToDetails(course);
    }

    public async Task UpdateAsync(int id, UpdateCourseDto dto)
    {
        var course = await _uow.Courses.GetByIdAsync(id) ?? throw new NotFoundException(nameof(Course), id);

        _ = await _uow.Categories.GetByIdAsync(dto.CategoryId)
            ?? throw new BadRequestException("Odabrana kategorija ne postoji.");

        course.Title = dto.Title;
        course.Description = dto.Description;
        course.InstructorName = dto.InstructorName;
        course.Price = dto.Price;
        course.CategoryId = dto.CategoryId;

        _uow.Courses.Update(course);
        await _uow.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var course = await _uow.Courses.GetByIdAsync(id) ?? throw new NotFoundException(nameof(Course), id);
        _uow.Courses.Remove(course);
        await _uow.SaveChangesAsync();
    }

    private static CourseDetailsDto MapToDetails(Course c) => new(
        c.Id, c.Title, c.Description, c.InstructorName, c.Price, c.CreatedAt,
        c.Category?.Name ?? string.Empty, c.CategoryId,
        c.Lessons.OrderBy(l => l.OrderIndex).Select(l => new LessonDto(l.Id, l.Title, l.Content, l.VideoUrl, l.OrderIndex, l.CourseId)).ToList(),
        c.Reviews.Select(r => new ReviewDto(r.Id, r.Rating, r.Comment, r.CreatedAt, r.User?.Username ?? string.Empty, r.CourseId)).ToList(),
        c.Reviews.Count > 0 ? Math.Round(c.Reviews.Average(r => r.Rating), 2) : 0,
        c.Enrollments.Count);
}
