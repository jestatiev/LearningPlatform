using LearningPlatform.Application.DTOs;
using LearningPlatform.Application.Exceptions;
using LearningPlatform.Application.Interfaces;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _uow;

    public UserService(IUnitOfWork uow) => _uow = uow;

    public async Task<List<UserDto>> GetAllAsync()
    {
        var users = await _uow.Users.Query()
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .ToListAsync();

        return users.Select(MapToDto).ToList();
    }

    public async Task<UserDto> GetByIdAsync(int id)
    {
        var user = await _uow.Users.Query()
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == id)
            ?? throw new NotFoundException(nameof(User), id);

        return MapToDto(user);
    }

    private static UserDto MapToDto(User u) => new(
        u.Id, u.Username, u.Email, u.FullName, u.CreatedAt,
        u.UserRoles.Select(ur => ur.Role.Name).ToList());
}
