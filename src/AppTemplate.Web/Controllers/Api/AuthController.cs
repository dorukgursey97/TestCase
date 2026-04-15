using AppTemplate.Application.DTOs.Auth;
using AppTemplate.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AppTemplate.Web.Controllers.Api;

public class AuthController : BaseApiController
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>Yeni kullanıcı kaydı oluşturur ve token döner.</summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(TokenResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        => ToResponse(await _authService.RegisterAsync(dto));

    /// <summary>Kullanıcı girişi yapar ve token döner.</summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(TokenResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
        => ToResponse(await _authService.LoginAsync(dto));

    /// <summary>Refresh token ile yeni access token alır.</summary>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(TokenResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto dto)
        => ToResponse(await _authService.RefreshAsync(dto.RefreshToken));
}
