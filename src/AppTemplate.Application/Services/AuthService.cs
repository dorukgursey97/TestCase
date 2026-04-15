using AppTemplate.Application.DTOs.Auth;
using AppTemplate.Application.Interfaces;
using AppTemplate.Domain.Common;
using AppTemplate.Domain.Entities;
using AppTemplate.Domain.Interfaces;
using System.Security.Cryptography;

namespace AppTemplate.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;

    public AuthService(IUnitOfWork unitOfWork, ITokenService tokenService)
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
    }

    public async Task<Result<TokenResponseDto>> RegisterAsync(RegisterDto dto)
    {
        var existing = await _unitOfWork.Repository<User>()
            .FindAsync(u => u.Email == dto.Email);

        if (existing.Any())
            return Result<TokenResponseDto>.Failure("This email address is already in use.");

        var user = new User
        {
            Username     = dto.Username,
            Email        = dto.Email,
            PasswordHash = HashPassword(dto.Password)
        };

        await _unitOfWork.Repository<User>().AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return await IssueTokensAsync(user);
    }

    public async Task<Result<TokenResponseDto>> LoginAsync(LoginDto dto)
    {
        var users = await _unitOfWork.Repository<User>()
            .FindAsync(u => u.Email == dto.Email);

        var user = users.FirstOrDefault();
        if (user is null || !VerifyPassword(dto.Password, user.PasswordHash))
            return Result<TokenResponseDto>.Failure("Invalid email or password.");

        return await IssueTokensAsync(user);
    }

    public async Task<Result<TokenResponseDto>> RefreshAsync(string refreshToken)
    {
        var users = await _unitOfWork.Repository<User>()
            .FindAsync(u => u.RefreshToken == refreshToken
                         && u.RefreshTokenExpiry > DateTime.UtcNow);

        var user = users.FirstOrDefault();
        if (user is null)
            return Result<TokenResponseDto>.Failure("Invalid or expired refresh token.");

        return await IssueTokensAsync(user);
    }

    private async Task<Result<TokenResponseDto>> IssueTokensAsync(User user)
    {
        user.RefreshToken       = _tokenService.GenerateRefreshToken();
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        user.UpdatedAt          = DateTime.UtcNow;

        await _unitOfWork.Repository<User>().UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return Result<TokenResponseDto>.Success(new TokenResponseDto
        {
            AccessToken  = _tokenService.GenerateAccessToken(user),
            RefreshToken = user.RefreshToken
        });
    }

    private static string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        using var rfc = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256);
        var hash = rfc.GetBytes(32);
        return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
    }

    private static bool VerifyPassword(string password, string stored)
    {
        var parts = stored.Split(':');
        if (parts.Length != 2) return false;

        var salt = Convert.FromBase64String(parts[0]);
        var expected = Convert.FromBase64String(parts[1]);

        using var rfc = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256);
        var actual = rfc.GetBytes(32);

        return CryptographicOperations.FixedTimeEquals(expected, actual);
    }
}
