using LearningPlatform.Application.DTOs;
using LearningPlatform.Application.Exceptions;
using LearningPlatform.Application.Interfaces;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Services;

public class LessonService : ILessonService
{
    private readonly IUnitOfWork _uow;

    public LessonService(IUnitOfWork uow) => _uow = uow;

    public async Task<List<LessonDto>> GetByCourseAsync(int courseId) =>
        (await _uow.Lessons.FindAsync(l => l.CourseId == courseId))
            .OrderBy(l => l.OrderIndex)
            .Select(l => new LessonDto(l.Id, l.Title, l.Content, l.VideoUrl, l.OrderIndex, l.CourseId))
            .ToList();

    public async Task<LessonDto> GetByIdAsync(int id)
    {
        var l = await _uow.Lessons.GetByIdAsync(id) ?? throw new NotFoundException(nameof(Lesson), id);
        return new LessonDto(l.Id, l.Title, l.Content, l.VideoUrl, l.OrderIndex, l.CourseId);
    }

    public async Task<LessonDto> CreateAsync(CreateLessonDto dto)
    {
        _ = await _uow.Courses.GetByIdAsync(dto.CourseId) ?? throw new BadRequestException("Tečaj ne postoji.");

        var lesson = new Lesson
        {
            Title = dto.Title,
            Content = dto.Content,
            VideoUrl = dto.VideoUrl,
            OrderIndex = dto.OrderIndex,
            CourseId = dto.CourseId,
        };
        await _uow.Lessons.AddAsync(lesson);
        await _uow.SaveChangesAsync();
        return new LessonDto(lesson.Id, lesson.Title, lesson.Content, lesson.VideoUrl, lesson.OrderIndex, lesson.CourseId);
    }

    public async Task UpdateAsync(int id, UpdateLessonDto dto)
    {
        var lesson = await _uow.Lessons.GetByIdAsync(id) ?? throw new NotFoundException(nameof(Lesson), id);
        lesson.Title = dto.Title;
        lesson.Content = dto.Content;
        lesson.VideoUrl = dto.VideoUrl;
        lesson.OrderIndex = dto.OrderIndex;
        _uow.Lessons.Update(lesson);
        await _uow.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var lesson = await _uow.Lessons.GetByIdAsync(id) ?? throw new NotFoundException(nameof(Lesson), id);
        _uow.Lessons.Remove(lesson);
        await _uow.SaveChangesAsync();
    }
}
