namespace JiraApp.Server.Services.Interfaces;

public interface IBoardsService
{
    Task<List<BoardDto>> GetAllBoardsAsync(CancellationToken token);
}
