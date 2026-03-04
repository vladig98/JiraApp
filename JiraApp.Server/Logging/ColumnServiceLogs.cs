namespace JiraApp.Server.Logging;

public static partial class ColumnServiceLogs
{
    // --- Information Level ---

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Creating column '{Name}' for Board '{BoardId}'.")]
    public static partial void LogColumnCreationStarted(this ILogger logger, string name, Guid boardId);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Successfully deleted column '{Id}' and shifted subsequent indices.")]
    public static partial void LogColumnDeleted(this ILogger logger, Guid id);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Reordered column '{Id}' from index {OldIndex} to {NewIndex}.")]
    public static partial void LogColumnReordered(this ILogger logger, Guid id, int oldIndex, int newIndex);

    // --- Debug Level ---

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Updating column '{Id}' metadata (Name: {Name}).")]
    public static partial void LogColumnUpdateStarted(this ILogger logger, Guid id, string name);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Shifting column indices for Board '{BoardId}' due to reorder. Range: {Min} to {Max}.")]
    public static partial void LogColumnShiftApplied(this ILogger logger, Guid boardId, int min, int max);

    // --- Warning Level ---

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Column operation aborted: Column '{Id}' not found.")]
    public static partial void LogColumnNotFound(this ILogger logger, Guid id);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Board '{BoardId}' not found during column creation.")]
    public static partial void LogBoardNotFoundColumn(this ILogger logger, Guid boardId);

    // --- Error Level ---

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Transaction failed during column creation for '{Name}': {Error}")]
    public static partial void LogColumnCreationError(this ILogger logger, string name, string error);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Failed to reorder column '{Id}': {Error}")]
    public static partial void LogColumnReorderError(this ILogger logger, Guid id, string error);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Critical error deleting column '{Id}': {Error}")]
    public static partial void LogColumnDeletionError(this ILogger logger, Guid id, string error);
}