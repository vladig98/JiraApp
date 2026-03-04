namespace JiraApp.Server.Logging;

public static partial class BoardsServiceLogs
{
    // --- Information Level (Happy Paths) ---

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Creating new board with name: {Name}")]
    public static partial void LogBoardCreationStarted(this ILogger logger, string name);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Successfully deleted board '{Id}' and re-indexed subsequent boards.")]
    public static partial void LogBoardDeleted(this ILogger logger, Guid id);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Updated board '{Id}' name to: {Name}")]
    public static partial void LogBoardUpdated(this ILogger logger, Guid id, string name);

    // --- Debug Level (High Volume / Detailed Flow) ---

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Fetching all boards from database (Ordered).")]
    public static partial void LogFetchingAllBoards(this ILogger logger);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Board '{Id}' found at index {OrderIndex}. Proceeding with operation.")]
    public static partial void LogBoardFound(this ILogger logger, Guid id, int orderIndex);

    // --- Warning Level (Expected Logic Failures) ---

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Board operation failed: Board '{Id}' not found.")]
    public static partial void LogBoardNotFound(this ILogger logger, Guid id);

    // --- Error Level (System/Database Failures) ---

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "An unexpected error occurred during board creation for '{Name}': {ErrorMessage}")]
    public static partial void LogBoardCreationError(this ILogger logger, string name, string errorMessage);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Critical failure deleting board '{Id}': {ErrorMessage}")]
    public static partial void LogBoardDeletionError(this ILogger logger, Guid id, string errorMessage);
}
