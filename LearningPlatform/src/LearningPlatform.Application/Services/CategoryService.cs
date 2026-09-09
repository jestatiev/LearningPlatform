using LearningPlatform.Application.DTOs;
using LearningPlatform.Application.Exceptions;
using LearningPlatform.Application.Interfaces;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _uow;

    public CategoryService(IUnitOfWork uow) => _uow = uow;

    public async Task<List<CategoryDto>> GetAllAsync()
    {
        var categories = await _uow.Categories.Query()
            .Include(c => c.Courses)
            .Select(c => new CategoryDto(c.Id, c.Name, c.Description, c.Courses.Count))
            .ToListAsync();
        return categories;
    }

    public async Task<CategoryDto> GetByIdAsync(int id)
    {
        var category = await _uow.Categories.Query()
            .Include(c => c.Courses)
            .FirstOrDefaultAsync(c => c.Id == id)
            ?? throw new NotFoundException(nameof(Category), id);

        return new CategoryDto(category.Id, category.Name, category.Description, category.Courses.Count);
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
    {
        var category = new Category { Name = dto.Name, Description = dto.Description };
        await _uow.Categories.AddAsync(category);
        await _uow.SaveChangesAsync();
        return new CategoryDto(category.Id, category.Name, category.Description, 0);
    }

    public async Task UpdateAsync(int id, CreateCategoryDto dto)
    {
        var category = await _uow.Categories.GetByIdAsync(id) ?? throw new NotFoundException(nameof(Category), id);
        category.Name = dto.Name;
        category.Description = dto.Description;
        _uow.Categories.Update(category);
        await _uow.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var category = await _uow.Categories.GetByIdAsync(id) ?? throw new NotFoundException(nameof(Category), id);
        _uow.Categories.Remove(category);
        await _uow.SaveChangesAsync();
    }
}
