namespace JiraApp.Server.Services;

public class BoardsService(MainDbContext mainDbContext) : IBoardsService
{
    public Task<List<BoardDto>> GetAllBoardsAsync(CancellationToken token) 
        => mainDbContext.Boards.AsNoTracking().Select(x => new BoardDto(x.Id, x.Name, x.OrderIndex)).ToListAsync(token);
}
