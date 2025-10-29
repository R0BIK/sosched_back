namespace SoschedBack.Core.Common.UnifiedResponse;

public class Result<T> : Result
{
    public T? Data { get; }

    internal Result(bool isSuccess, Error error, T? data)
        : base(isSuccess, error)
    {
        Data = data;
    }
    
    public T ValueOrThrow()
    {
        if (!IsSuccess)
            throw new InvalidOperationException($"Cannot access data. Result is failure: {Error}");
        return Data!;
    }

    public override string ToString()
    {
        return IsSuccess ? $"Success: {Data}" : $"Failure: {Error}";
    }
}