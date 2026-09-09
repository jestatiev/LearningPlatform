using LearningPlatform.Application.DTOs;
using LearningPlatform.Application.Exceptions;
using LearningPlatform.Application.Interfaces;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Repositories;

namespace LearningPlatform.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _uow;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthService(IUnitOfWork uow, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator)
    {
        _uow = uow;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        if (await _uow.Users.UsernameExistsAsync(dto.Username))
            throw new ConflictException("Korisničko ime je već zauzeto.");

        if (await _uow.Users.EmailExistsAsync(dto.Email))
            throw new ConflictException("Email je već registriran.");

        _passwordHasher.CreatePasswordHash(dto.Password, out var hash, out var salt);

        var studentRole = (await _uow.Roles.FindAsync(r => r.Name == "Student")).FirstOrDefault()
            ?? throw new Exception("Uloga 'Student' nije pronađena u bazi. Provjerite seed podatke.");

        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            FullName = dto.FullName,
            PasswordHash = hash,
            PasswordSalt = salt,
        };
        user.UserRoles.Add(new UserRole { Role = studentRole });

        await _uow.Users.AddAsync(user);
        await _uow.SaveChangesAsync();

        var (token, expiresAt) = _jwtTokenGenerator.GenerateToken(user, new[] { studentRole.Name });
        return new AuthResponseDto(token, expiresAt, user.Username, new List<string> { studentRole.Name });
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _uow.Users.GetByUsernameWithRolesAsync(dto.Username)
            ?? throw new BadRequestException("Pogrešno korisničko ime ili lozinka.");

        if (!_passwordHasher.VerifyPassword(dto.Password, user.PasswordHash, user.PasswordSalt))
            throw new BadRequestException("Pogrešno korisničko ime ili lozinka.");

        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
        var (token, expiresAt) = _jwtTokenGenerator.GenerateToken(user, roles);

        return new AuthResponseDto(token, expiresAt, user.Username, roles);
    }
}
