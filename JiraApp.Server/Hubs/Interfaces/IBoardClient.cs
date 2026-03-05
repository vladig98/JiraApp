namespace JiraApp.Server.Hubs.Interfaces;

public interface IBoardClient
{
    /// <summary> Broadcasts a full Board object upon creation. </summary>
    Task ReceiveBoardCreated(BoardDto board);

    /// <summary> Broadcasts updated Board metadata (Name, OrderIndex). </summary>
    Task ReceiveBoardUpdated(BoardDto board);

    /// <summary> Notifies clients to remove a board from the UI cache by Id. </summary>
    Task ReceiveBoardDeleted(Guid id);
}
