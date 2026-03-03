namespace JiraApp.Server.Services;

public class BoardsService(MainDbContext mainDbContext) : IBoardsService
{
    public async Task<Result<BoardDto>> CreateBoardAsync(CreateBoardDto createBoardDto, CancellationToken ct)
    {
        DateTime now = DateTime.UtcNow;
        int currentCount = await mainDbContext.Boards.CountAsync(ct);

        BoardModel board = new()
        {
            Name = createBoardDto.Name,
            OrderIndex = currentCount,
            CreatedAt = now,
            UpdatedAt = now
        };

        await mainDbContext.Boards.AddAsync(board, ct);
        await mainDbContext.SaveChangesAsync(ct);

        return new BoardDto(board.Id, board.Name, board.OrderIndex, now, now);
    }

    public async Task<BaseResult> DeleteBoardAsync(Guid id, CancellationToken ct)
    {
        int deletedEntries = await mainDbContext.Boards.Where(x => x.Id == id).ExecuteDeleteAsync(ct);
        if (deletedEntries == 0)
        {
            return BaseResult.Failure($"Board with {id} not found.", ErrorType.NotFound);
        }

        List<BoardModel> boards = await mainDbContext.Boards.OrderBy(x => x.OrderIndex).ToListAsync(ct);
        int orderIndex = 0;

        foreach (BoardModel board in boards)
        {
            board.OrderIndex = orderIndex++;
        }

        await mainDbContext.SaveChangesAsync(ct);

        return BaseResult.Success();
    }

    public async Task<IReadOnlyList<BoardDto>> GetAllBoardsAsync(CancellationToken token)
        => await mainDbContext.Boards
            .AsNoTracking()
            .Select(x => new BoardDto(x.Id, x.Name, x.OrderIndex, x.CreatedAt, x.UpdatedAt))
            .ToListAsync(token);

    public async Task<Result<BoardDto>> UpdateBoardAsync(EditBoardDto editBoardDto, Guid id, CancellationToken ct)
    {
        BoardModel? board = await mainDbContext.Boards.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (board is null)
        {
            return Result<BoardDto>.Failure($"Board with {id} not found.", ErrorType.NotFound);
        }

        DateTime now = DateTime.UtcNow;

        board.Name = editBoardDto.Name;
        board.UpdatedAt = now;

        await mainDbContext.SaveChangesAsync(ct);
        return new BoardDto(board.Id, board.Name, board.OrderIndex, board.CreatedAt, now);
    }
}
