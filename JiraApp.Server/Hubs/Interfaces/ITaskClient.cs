namespace JiraApp.Server.Hubs.Interfaces;

public interface ITaskClient
{
    /// <summary> Broadcasts a new task added to a column. </summary>
    Task ReceiveTaskCreated(TaskDto task);

    /// <summary> Broadcasts updates to Title or Description. </summary>
    Task ReceiveTaskUpdated(TaskDto task);

    /// <summary> Notifies clients to purge a task from the UI. </summary>
    Task ReceiveTaskDeleted(Guid id);

    /// <summary> 
    /// Critical for Drag & Drop. Broadcasts updated OrderIndex and ColumnId 
    /// to ensure frontend state matches the database. 
    /// </summary>
    Task ReceiveTaskMoved(TaskDto task);
}
