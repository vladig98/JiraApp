namespace JiraApp.Server.Hubs.Interfaces;

public interface IColumnClient
{
    /// <summary> Broadcasts a new column within a specific board context. </summary>
    Task ReceiveColumnCreated(ColumnDto column);

    /// <summary> Broadcasts changes to column title or metadata. </summary>
    Task ReceiveColumnUpdated(ColumnDto column);

    /// <summary> Notifies clients to remove a column from a board. </summary>
    Task ReceiveColumnDeleted(Guid id);

    /// <summary> Broadcasts new OrderIndex for reconciliation during Drag & Drop. </summary>
    Task ReceiveColumnMoved(ColumnDto column);
}
