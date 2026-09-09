using LearningPlatform.Application.Interfaces;
using LearningPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context, IPasswordHasher passwordHasher)
    {
        await context.Database.MigrateAsync();

        if (!await context.Roles.AnyAsync())
        {
            context.Roles.AddRange(
                new Role { Name = "Admin" },
                new Role { Name = "Student" });
            await context.SaveChangesAsync();
        }

        if (!await context.Users.AnyAsync())
        {
            var adminRole = await context.Roles.FirstAsync(r => r.Name == "Admin");
            passwordHasher.CreatePasswordHash("Admin123!", out var hash, out var salt);

            var admin = new User
            {
                Username = "admin",
                Email = "admin@learningplatform.local",
                FullName = "Administrator",
                PasswordHash = hash,
                PasswordSalt = salt,
            };
            admin.UserRoles.Add(new UserRole { Role = adminRole });
            context.Users.Add(admin);
            await context.SaveChangesAsync();
        }

        if (!await context.Categories.AnyAsync())
        {
            context.Categories.AddRange(
                new Category { Name = "Programiranje", Description = "Tečajevi vezani uz razvoj softvera" },
                new Category { Name = "Dizajn", Description = "Grafički i UX/UI dizajn" },
                new Category { Name = "Poslovanje", Description = "Menadžment, marketing i financije" });
            await context.SaveChangesAsync();
        }
    }
}
