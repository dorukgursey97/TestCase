using AppTemplate.Application.DTOs.Todo;
using AppTemplate.Application.Services;
using Xunit;
using AppTemplate.Domain.Entities;
using AppTemplate.Domain.Interfaces;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;

namespace AppTemplate.Application.Tests.Services;

public class TodoServiceDeleteTests
{
    private readonly Mock<IUnitOfWork>              _unitOfWork       = new();
    private readonly Mock<IRepository<TodoItem>>   _todoRepo         = new();
    private readonly Mock<IValidator<CreateTodoDto>> _createValidator = new();
    private readonly Mock<IValidator<UpdateTodoDto>> _updateValidator = new();
    private readonly Mock<ILogger<TodoService>>     _logger           = new();

    private readonly TodoService _sut;

    public TodoServiceDeleteTests()
    {
        _unitOfWork.Setup(u => u.Repository<TodoItem>()).Returns(_todoRepo.Object);

        _sut = new TodoService(
            _unitOfWork.Object,
            _createValidator.Object,
            _updateValidator.Object,
            _logger.Object);
    }

    // ── Happy path ────────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_WhenTodoExistsAndBelongsToUser_ReturnsSuccess()
    {
        // Arrange
        var todo = new TodoItem { Id = 1, UserId = 42, Title = "Test", IsDeleted = false };
        _todoRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(todo);

        // Act
        var result = await _sut.DeleteAsync(1, userId: 42);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAsync_WhenTodoExistsAndBelongsToUser_SetsIsDeletedTrue()
    {
        // Arrange
        var todo = new TodoItem { Id = 1, UserId = 42, Title = "Test", IsDeleted = false };
        _todoRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(todo);

        // Act
        await _sut.DeleteAsync(1, userId: 42);

        // Assert
        todo.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAsync_WhenTodoExistsAndBelongsToUser_CallsUpdateAndSave()
    {
        // Arrange
        var todo = new TodoItem { Id = 1, UserId = 42, Title = "Test" };
        _todoRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(todo);

        // Act
        await _sut.DeleteAsync(1, userId: 42);

        // Assert
        _todoRepo.Verify(r => r.UpdateAsync(todo), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    // ── Not found ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_WhenTodoDoesNotExist_ReturnsFailure()
    {
        // Arrange
        _todoRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((TodoItem?)null);

        // Act
        var result = await _sut.DeleteAsync(1, userId: 42);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Todo not found.");
    }

    [Fact]
    public async Task DeleteAsync_WhenTodoDoesNotExist_DoesNotCallSave()
    {
        // Arrange
        _todoRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((TodoItem?)null);

        // Act
        await _sut.DeleteAsync(1, userId: 42);

        // Assert
        _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    // ── Wrong user ────────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_WhenTodoBelongsToDifferentUser_ReturnsFailure()
    {
        // Arrange
        var todo = new TodoItem { Id = 1, UserId = 99, Title = "Test" };
        _todoRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(todo);

        // Act
        var result = await _sut.DeleteAsync(1, userId: 42);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Todo not found.");
    }

    [Fact]
    public async Task DeleteAsync_WhenTodoBelongsToDifferentUser_DoesNotCallSave()
    {
        // Arrange
        var todo = new TodoItem { Id = 1, UserId = 99, Title = "Test" };
        _todoRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(todo);

        // Act
        await _sut.DeleteAsync(1, userId: 42);

        // Assert
        _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    // ── Exception ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_WhenRepositoryThrows_ReturnsFailure()
    {
        // Arrange
        _todoRepo.Setup(r => r.GetByIdAsync(1)).ThrowsAsync(new Exception("DB error"));

        // Act
        var result = await _sut.DeleteAsync(1, userId: 42);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("An error occurred while deleting the todo.");
    }

    [Fact]
    public async Task DeleteAsync_WhenRepositoryThrows_LogsError()
    {
        // Arrange
        _todoRepo.Setup(r => r.GetByIdAsync(1)).ThrowsAsync(new Exception("DB error"));

        // Act
        await _sut.DeleteAsync(1, userId: 42);

        // Assert
        _logger.Verify(
            l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Error deleting todo")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
