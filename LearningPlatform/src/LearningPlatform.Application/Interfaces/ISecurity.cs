using LearningPlatform.Domain.Entities;

namespace LearningPlatform.Application.Interfaces;

public interface IPasswordHasher
{
    void CreatePasswordHash(string password, out byte[] hash, out byte[] salt);
    bool VerifyPassword(string password, byte[] hash, byte[] salt);
}

public interface IJwtTokenGenerator
{
    (string token, DateTime expiresAt) GenerateToken(User user, IEnumerable<string> roles);
}
