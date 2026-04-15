namespace AppTemplate.Application.DTOs.Todo;

public class UpdateTodoDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsCompleted { get; set; }
}
