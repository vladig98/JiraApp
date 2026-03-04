namespace JiraApp.Server.Hubs.Interfaces;

public interface ITaskClient
{
    Task ReceiveTaskCreated(TaskDto task);
    Task ReceiveTaskUpdated(TaskDto task);
    Task ReceiveTaskDeleted(Guid id);
    Task ReceiveTaskMoved(TaskDto task);
}
