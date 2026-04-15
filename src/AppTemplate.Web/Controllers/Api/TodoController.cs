using AppTemplate.Application.DTOs.Todo;
using AppTemplate.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AppTemplate.Web.Controllers.Api;

[Authorize]
public class TodoController : BaseApiController
{
    private readonly ITodoService _todoService;

    public TodoController(ITodoService todoService)
    {
        _todoService = todoService;
    }

    private int CurrentUserId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>Giriş yapan kullanıcının tüm görevlerini listeler.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TodoResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
        => ToResponse(await _todoService.GetAllAsync(CurrentUserId));

    /// <summary>ID'ye göre görev getirir.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TodoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
        => ToResponse(await _todoService.GetByIdAsync(id, CurrentUserId));

    /// <summary>Yeni görev oluşturur.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(TodoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateTodoDto dto)
        => ToResponse(await _todoService.CreateAsync(dto, CurrentUserId));

    /// <summary>Görevi günceller.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(TodoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTodoDto dto)
        => ToResponse(await _todoService.UpdateAsync(id, dto, CurrentUserId));

    /// <summary>Görevi siler (soft delete).</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
        => ToResponse(await _todoService.DeleteAsync(id, CurrentUserId));
}
