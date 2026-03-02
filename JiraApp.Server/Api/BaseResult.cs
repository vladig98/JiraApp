namespace JiraApp.Server.Api;

public class BaseResult
{
    private protected BaseResult(bool isSuccess, string? error, ErrorType errorType)
    {
        Error = error;
        ErrorType = errorType;
        IsSuccess = isSuccess;
    }

    public string? Error { get; }
    public ErrorType ErrorType { get; }
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    public static BaseResult Failure(string error, ErrorType type)
    {
        return new BaseResult(isSuccess: false, error, type);
    }

    public static BaseResult Success()
    {
        return new BaseResult(isSuccess: true, null, ErrorType.None);
    }
}
