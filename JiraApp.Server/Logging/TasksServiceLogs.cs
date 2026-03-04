namespace JiraApp.Server.Logging;

public static partial class TasksServiceLogs
{
    // --- Information Level ---
    [LoggerMessage(Level = LogLevel.Information, Message = "Creating task '{Title}' in column '{ColumnId}'.")]
    public static partial void LogTaskCreationStarted(this ILogger logger, string title, Guid columnId);

    [LoggerMessage(Level = LogLevel.Information, Message = "Task '{Id}' successfully deleted from column '{ColumnId}'.")]
    public static partial void LogTaskDeleted(this ILogger logger, Guid id, Guid columnId);

    [LoggerMessage(Level = LogLevel.Information, Message = "Task '{Id}' moved from Column '{SourceColumnId}' to '{DestColumnId}' at index {NewIndex}.")]
    public static partial void LogTaskMovedAcrossColumns(this ILogger logger, Guid id, Guid sourceColumnId, Guid destColumnId, int newIndex);

    [LoggerMessage(Level = LogLevel.Information, Message = "Task '{Id}' reordered within column from index {OldIndex} to {NewIndex}.")]
    public static partial void LogTaskReordered(this ILogger logger, Guid id, int oldIndex, int newIndex);

    // --- Debug Level ---
    [LoggerMessage(Level = LogLevel.Debug, Message = "Updating task '{Id}' content (Title: {Title}).")]
    public static partial void LogTaskUpdateStarted(this ILogger logger, Guid id, string title);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Shifting task indices in column '{ColumnId}'. Range: {Min} to {Max}. Direction: {Direction}.")]
    public static partial void LogTaskShiftApplied(this ILogger logger, Guid columnId, int min, int max, string direction);

    // --- Warning Level ---
    [LoggerMessage(Level = LogLevel.Warning, Message = "Task operation failed: Task '{Id}' not found.")]
    public static partial void LogTaskNotFound(this ILogger logger, Guid id);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Column '{ColumnId}' not found for task operation.")]
    public static partial void LogColumnNotFoundForTask(this ILogger logger, Guid columnId);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Concurrency conflict for Task '{Id}'. Someone else modified it.")]
    public static partial void LogTaskConcurrencyConflict(this ILogger logger, Guid id);

    // --- Error Level ---
    [LoggerMessage(Level = LogLevel.Error, Message = "Critical error during task creation '{Title}': {Error}")]
    public static partial void LogTaskCreationError(this ILogger logger, string title, string error);

    [LoggerMessage(Level = LogLevel.Error, Message = "Failed to move/reorder task '{Id}': {Error}")]
    public static partial void LogTaskMoveError(this ILogger logger, Guid id, string error);

    [LoggerMessage(Level = LogLevel.Error, Message = "Failed to update task '{Id}': {Error}")]
    public static partial void LogTaskUpdateError(this ILogger logger, Guid id, string error);

    [LoggerMessage(Level = LogLevel.Error, Message = "Failed to delete task '{Id}': {Error}")]
    public static partial void LogTaskDeleteError(this ILogger logger, Guid id, string error);
}