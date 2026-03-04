namespace JiraApp.Server.Hubs.Interfaces;

public interface IBoardClient
{
    Task ReceiveBoardCreated(BoardDto board);
    Task ReceiveBoardUpdated(BoardDto board);
    Task ReceiveBoardDeleted(Guid id);
}
