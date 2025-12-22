namespace Application.Common;
public class Result
{
    public bool IsSuccess { get; }
    public string Error { get; }
    public string? Message { get; }

    public Result(bool isSuccess, string error = "", string? message = null)
    {
        IsSuccess = isSuccess;
        Error = error;
        Message = message;
    }

    public static Result Success(string? message = null) => new Result(true, message: message);
    public static Result Failure(string error) => new Result(false, error);
}

public class Result<T> : Result
{
    public T Value { get; }

    public Result(T value, bool isSuccess, string error = "", string? message = null)
        : base(isSuccess, error, message)
    {
        Value = value;
    }

    public static Result<T> Success(T value, string? message = null) => new Result<T>(value, true, message: message);
    public static Result<T> Failure(string error) => new Result<T>(default!, false, error);
}
