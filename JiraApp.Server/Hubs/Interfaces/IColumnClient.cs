namespace JiraApp.Server.Hubs.Interfaces;

public interface IColumnClient
{
    Task ReceiveColumnCreated(ColumnDto column);
    Task ReceiveColumnUpdated(ColumnDto column);
    Task ReceiveColumnDeleted(Guid id);
    Task ReceiveColumnMoved(ColumnDto column);
}
