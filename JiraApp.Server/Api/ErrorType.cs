namespace JiraApp.Server.Api;

public enum ErrorType
{
    None = 0,
    NotFound = 1,
    Concurrency = 2,
    Unexpected = 3
}
