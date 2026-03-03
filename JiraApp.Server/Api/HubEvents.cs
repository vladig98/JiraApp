namespace JiraApp.Server.Api;

public enum HubEvents
{
    ReceiveTaskCreated,
    ReceiveTaskUpdated,
    ReceiveTaskDeleted,
    ReceiveTaskMoved,
    ReceiveColumnReordered
}
