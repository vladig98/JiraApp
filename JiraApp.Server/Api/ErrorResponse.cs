namespace JiraApp.Server.Api;

public readonly record struct ErrorResponse(int ResponseCode, string ResponseName, IEnumerable<string> Errors);