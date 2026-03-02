namespace JiraApp.Server.Api;

public class Result<T> : BaseResult
{
    private Result(T? data, bool isSuccess, string? error, ErrorType errorType) : base(isSuccess, error, errorType)
    {
        Data = data;
    }

    public T? Data { get; }

    public new static Result<T> Failure(string error, ErrorType type)
    {
        return new Result<T>(default, isSuccess: false, error, type);
    }

    public static Result<T> Success(T data)
    {
        return new Result<T>(data, isSuccess: true, null, ErrorType.None);
    }

    public static implicit operator Result<T>(T data)
    {
        return Success(data);
    }
}
