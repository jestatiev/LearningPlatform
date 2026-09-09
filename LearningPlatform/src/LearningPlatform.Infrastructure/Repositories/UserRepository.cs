using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Repositories;
using LearningPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Infrastructure.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context) { }

    public async Task<User?> GetByUsernameWithRolesAsync(string username) =>
        await DbSet
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Username == username);

    public async Task<bool> UsernameExistsAsync(string username) =>
        await DbSet.AnyAsync(u => u.Username == username);

    public async Task<bool> EmailExistsAsync(string email) =>
        await DbSet.AnyAsync(u => u.Email == email);
}
