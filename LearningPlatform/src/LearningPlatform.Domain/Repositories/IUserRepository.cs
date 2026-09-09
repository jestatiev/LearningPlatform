using LearningPlatform.Domain.Entities;

namespace LearningPlatform.Domain.Repositories;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByUsernameWithRolesAsync(string username);
    Task<bool> UsernameExistsAsync(string username);
    Task<bool> EmailExistsAsync(string email);
}
