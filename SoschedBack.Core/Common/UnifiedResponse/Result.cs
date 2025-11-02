namespace SoschedBack.Core.Common.UnifiedResponse;

public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }
    
    public Result(bool isSuccess, Error error)
    {
        IsSuccess = isSuccess;
        Error = error ?? throw new ArgumentNullException(nameof(error));
    }

    public static Result Success() => new(true, Error.None);
    public static Result Failure(Error error) => new(false, error);

    public static Result<T> Success<T>(T data) => new(true, Error.None, data);
    public static Result<T> Failure<T>(Error error) => new(false, error, default);

    public override string ToString()
    {
        return IsSuccess ? "Success" : $"Failure: {Error}";
    }
}
