namespace JiraApp.Server.Services;

public partial class BoardsService(
    MainDbContext mainDbContext,
    ILogger<BoardsService> logger) : IBoardsService
{
    public async Task<Result<BoardDto>> CreateBoardAsync(CreateBoardDto createBoardDto, CancellationToken ct)
    {
        logger.LogBoardCreationStarted(createBoardDto.Name);

        using IDbContextTransaction transaction = await mainDbContext.Database.BeginTransactionAsync(ct);
        try
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

            await transaction.CommitAsync(ct);

            return new BoardDto(board.Id, board.Name, board.OrderIndex, now, now);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(ct);
            logger.LogBoardCreationError(createBoardDto.Name, ex.Message);

            return Result<BoardDto>.Failure("An error occurred during creation.", ErrorType.Unexpected);
        }
    }

    public async Task<BaseResult> DeleteBoardAsync(Guid id, CancellationToken ct)
    {
        int? orderIndex = await mainDbContext.Boards
            .Where(x => x.Id == id)
            .Select(x => (int?)x.OrderIndex)
            .FirstOrDefaultAsync(ct);

        if (!orderIndex.HasValue)
        {
            logger.LogBoardNotFound(id);
            return BaseResult.Failure($"Board '{id}' not found.", ErrorType.NotFound);
        }

        logger.LogBoardFound(id, orderIndex.Value);

        using IDbContextTransaction transaction = await mainDbContext.Database.BeginTransactionAsync(ct);
        try
        {
            await mainDbContext.Boards
                .Where(x => x.Id == id)
                .ExecuteDeleteAsync(ct);

            // Shift the indices of all subsequent boards down by 1
            await mainDbContext.Boards
                .Where(x => x.OrderIndex > orderIndex.Value)
                .ExecuteUpdateAsync(setter => setter.SetProperty(
                    x => x.OrderIndex,
                    x => x.OrderIndex - 1),
                ct);

            await transaction.CommitAsync(ct);
            logger.LogBoardDeleted(id);

            return BaseResult.Success();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(ct);
            logger.LogBoardDeletionError(id, ex.Message);

            return BaseResult.Failure("An error occurred during deletion.", ErrorType.Unexpected);
        }
    }

    public async Task<IReadOnlyList<BoardDto>> GetAllBoardsAsync(CancellationToken token)
    {
        logger.LogFetchingAllBoards();
        return await mainDbContext.Boards
                .AsNoTracking()
                .OrderBy(x => x.OrderIndex)
                .Select(x => new BoardDto(x.Id, x.Name, x.OrderIndex, x.CreatedAt, x.UpdatedAt))
                .ToListAsync(token);
    }

    public async Task<Result<BoardDto>> UpdateBoardAsync(EditBoardDto editBoardDto, Guid id, CancellationToken ct)
    {
        BoardModel? board = await mainDbContext.Boards.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (board is null)
        {
            return Result<BoardDto>.Failure($"Board '{id}' not found.", ErrorType.NotFound);
        }

        board.Name = editBoardDto.Name;
        board.UpdatedAt = DateTime.UtcNow;

        await mainDbContext.SaveChangesAsync(ct);
        logger.LogBoardUpdated(id, editBoardDto.Name);

        return new BoardDto(board.Id, board.Name, board.OrderIndex, board.CreatedAt, board.UpdatedAt);
    }
}
