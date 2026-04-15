using AppTemplate.Application.DTOs.Auth;
using AppTemplate.Domain.Common;

namespace AppTemplate.Application.Interfaces;

public interface IAuthService
{
    Task<Result<TokenResponseDto>> RegisterAsync(RegisterDto dto);
    Task<Result<TokenResponseDto>> LoginAsync(LoginDto dto);
    Task<Result<TokenResponseDto>> RefreshAsync(string refreshToken);
}
