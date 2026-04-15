using AppTemplate.Application.DTOs.Todo;
using AppTemplate.Domain.Common;

namespace AppTemplate.Application.Interfaces;

public interface ITodoService
{
    Task<Result<IEnumerable<TodoResponseDto>>> GetAllAsync(int userId);
    Task<Result<TodoResponseDto>> GetByIdAsync(int id, int userId);
    Task<Result<TodoResponseDto>> CreateAsync(CreateTodoDto dto, int userId);
    Task<Result<TodoResponseDto>> UpdateAsync(int id, UpdateTodoDto dto, int userId);
    Task<Result> DeleteAsync(int id, int userId);
}
