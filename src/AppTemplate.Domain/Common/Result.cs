namespace AppTemplate.Domain.Common;

/// <summary>
/// Generic Result pattern — service katmanından hata/başarı dönerken kullanılır.
/// </summary>
public class Result<T>
{
    public bool IsSuccess { get; private set; }
    public T? Value { get; private set; }
    public string? Error { get; private set; }
    public List<string> Errors { get; private set; } = new();

    private Result(bool isSuccess, T? value, string? error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    public static Result<T> Success(T value) => new(true, value, null);

    public static Result<T> Failure(string error) => new(false, default, error);

    public static Result<T> Failure(List<string> errors)
    {
        var result = new Result<T>(false, default, errors.FirstOrDefault());
        result.Errors = errors;
        return result;
    }
}

public class Result
{
    public bool IsSuccess { get; private set; }
    public string? Error { get; private set; }
    public List<string> Errors { get; private set; } = new();

    private Result(bool isSuccess, string? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, null);

    public static Result Failure(string error) => new(false, error);

    public static Result Failure(List<string> errors)
    {
        var result = new Result(false, errors.FirstOrDefault());
        result.Errors = errors;
        return result;
    }
}
