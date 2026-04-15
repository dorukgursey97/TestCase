using AppTemplate.Application.DTOs.Todo;
using AppTemplate.Application.Interfaces;
using AppTemplate.Domain.Common;
using AppTemplate.Domain.Entities;
using AppTemplate.Domain.Interfaces;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace AppTemplate.Application.Services;

public class TodoService : ITodoService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateTodoDto> _createValidator;
    private readonly IValidator<UpdateTodoDto> _updateValidator;
    private readonly ILogger<TodoService> _logger;

    public TodoService(
        IUnitOfWork unitOfWork,
        IValidator<CreateTodoDto> createValidator,
        IValidator<UpdateTodoDto> updateValidator,
        ILogger<TodoService> logger)
    {
        _unitOfWork = unitOfWork;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<TodoResponseDto>>> GetAllAsync(int userId)
    {
        try
        {
            var todos = await _unitOfWork.Repository<TodoItem>()
                .FindAsync(t => t.UserId == userId);

            return Result<IEnumerable<TodoResponseDto>>.Success(todos.Select(MapToResponse));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching todos for user {UserId}", userId);
            return Result<IEnumerable<TodoResponseDto>>.Failure("An error occurred while fetching todos.");
        }
    }

    public async Task<Result<TodoResponseDto>> GetByIdAsync(int id, int userId)
    {
        try
        {
            var todo = await _unitOfWork.Repository<TodoItem>().GetByIdAsync(id);
            if (todo is null || todo.UserId != userId)
                return Result<TodoResponseDto>.Failure("Todo not found.");

            return Result<TodoResponseDto>.Success(MapToResponse(todo));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching todo {Id}", id);
            return Result<TodoResponseDto>.Failure("An error occurred while fetching the todo.");
        }
    }

    public async Task<Result<TodoResponseDto>> CreateAsync(CreateTodoDto dto, int userId)
    {
        var validation = await _createValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return Result<TodoResponseDto>.Failure(
                validation.Errors.Select(e => e.ErrorMessage).ToList());

        try
        {
            var todo = new TodoItem
            {
                Title = dto.Title,
                Description = dto.Description,
                UserId = userId
            };

            await _unitOfWork.Repository<TodoItem>().AddAsync(todo);
            await _unitOfWork.SaveChangesAsync();

            return Result<TodoResponseDto>.Success(MapToResponse(todo));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating todo");
            return Result<TodoResponseDto>.Failure("An error occurred while creating the todo.");
        }
    }

    public async Task<Result<TodoResponseDto>> UpdateAsync(int id, UpdateTodoDto dto, int userId)
    {
        var validation = await _updateValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return Result<TodoResponseDto>.Failure(
                validation.Errors.Select(e => e.ErrorMessage).ToList());

        try
        {
            var todo = await _unitOfWork.Repository<TodoItem>().GetByIdAsync(id);
            if (todo is null || todo.UserId != userId)
                return Result<TodoResponseDto>.Failure("Todo not found.");

            todo.Title = dto.Title;
            todo.Description = dto.Description;
            todo.IsCompleted = dto.IsCompleted;
            todo.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Repository<TodoItem>().UpdateAsync(todo);
            await _unitOfWork.SaveChangesAsync();

            return Result<TodoResponseDto>.Success(MapToResponse(todo));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating todo {Id}", id);
            return Result<TodoResponseDto>.Failure("An error occurred while updating the todo.");
        }
    }

    public async Task<Result> DeleteAsync(int id, int userId)
    {
        try
        {
            var todo = await _unitOfWork.Repository<TodoItem>().GetByIdAsync(id);
            if (todo is null || todo.UserId != userId)
                return Result.Failure("Todo not found.");

            todo.IsDeleted = true;
            todo.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Repository<TodoItem>().UpdateAsync(todo);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting todo {Id}", id);
            return Result.Failure("An error occurred while deleting the todo.");
        }
    }

    private static TodoResponseDto MapToResponse(TodoItem t) => new()
    {
        Id          = t.Id,
        Title       = t.Title,
        Description = t.Description,
        IsCompleted = t.IsCompleted,
        CreatedAt   = t.CreatedAt,
        UpdatedAt   = t.UpdatedAt
    };
}
