namespace JiraApp.Server.Services;

public class ColumnService(
    MainDbContext mainDbContext,
    ILogger<ColumnService> logger) : IColumnService
{
    public async Task<Result<ColumnDto>> CreateColumnAsync(Guid boardId, CreateColumnDto createColumnDto, CancellationToken ct)
    {
        logger.LogColumnCreationStarted(createColumnDto.Name, boardId);

        bool boardExists = await mainDbContext.Boards.AnyAsync(x => x.Id == boardId, ct);
        if (!boardExists)
        {
            logger.LogBoardNotFoundColumn(boardId);
            return Result<ColumnDto>.Failure($"Board '{boardId}' not found.", ErrorType.NotFound);
        }

        await using IDbContextTransaction transaction = await mainDbContext.Database.BeginTransactionAsync(ct);
        try
        {
            DateTime now = DateTime.UtcNow;
            int currentCount = await mainDbContext.Columns.Where(x => x.BoardId == boardId).CountAsync(ct);

            ColumnModel column = new()
            {
                Name = createColumnDto.Name,
                BoardId = boardId,
                CreatedAt = now,
                UpdatedAt = now,
                OrderIndex = currentCount
            };

            await mainDbContext.Columns.AddAsync(column, ct);
            await mainDbContext.SaveChangesAsync(ct);

            await transaction.CommitAsync(ct);

            return new ColumnDto(column.Id, column.Name, column.OrderIndex, column.CreatedAt, column.UpdatedAt);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(ct);
            logger.LogColumnCreationError(createColumnDto.Name, ex.Message);

            return Result<ColumnDto>.Failure("An error occurred during creation.", ErrorType.Unexpected);
        }
    }

    public async Task<BaseResult> DeleteColumnAsync(Guid id, CancellationToken ct)
    {
        var columnInfo = await mainDbContext.Columns
                .Where(x => x.Id == id)
                .Select(x => new { x.BoardId, x.OrderIndex })
                .FirstOrDefaultAsync(ct);

        if (columnInfo is null)
        {
            logger.LogColumnNotFound(id);
            return BaseResult.Failure($"Column '{id}' not found.", ErrorType.NotFound);
        }

        await using IDbContextTransaction transaction = await mainDbContext.Database.BeginTransactionAsync(ct);

        try
        {
            await mainDbContext.Columns
                .Where(x => x.Id == id)
                .ExecuteDeleteAsync(ct);

            // Shift the indices of all subsequent boards down by 1
            await mainDbContext.Columns
                .Where(x => x.BoardId == columnInfo.BoardId && x.OrderIndex > columnInfo.OrderIndex)
                .ExecuteUpdateAsync(setter => setter.SetProperty(
                    x => x.OrderIndex,
                    x => x.OrderIndex - 1),
                ct);

            await transaction.CommitAsync(ct);
            logger.LogColumnDeleted(id);

            return BaseResult.Success();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(ct);
            logger.LogColumnDeletionError(id, ex.Message);

            return Result<ColumnDto>.Failure("An error occurred during deletion.", ErrorType.Unexpected);
        }
    }

    public async Task<Result<ColumnDto>> UpdateColumnAsync(Guid id, EditColumnDto editColumnDto, CancellationToken ct)
    {
        logger.LogColumnUpdateStarted(id, editColumnDto.Name);
        ColumnModel? column = await mainDbContext.Columns.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (column is null)
        {
            logger.LogColumnNotFound(id);
            return Result<ColumnDto>.Failure($"Column '{id}' not found.", ErrorType.NotFound);
        }

        column.Name = editColumnDto.Name;
        column.UpdatedAt = DateTime.UtcNow;

        await mainDbContext.SaveChangesAsync(ct);

        return new ColumnDto(column.Id, column.Name, column.OrderIndex, column.CreatedAt, column.UpdatedAt);
    }

    public async Task<Result<ColumnDto>> UpdateColumnOrderAsync(ReorderColumnDto reorderColumnDto, CancellationToken ct)
    {
        ColumnModel? column = await mainDbContext.Columns.FirstOrDefaultAsync(x => x.Id == reorderColumnDto.Id, ct);
        if (column is null)
        {
            logger.LogColumnNotFound(reorderColumnDto.Id);
            return Result<ColumnDto>.Failure($"Column '{reorderColumnDto.Id}' not found.", ErrorType.NotFound);
        }

        int oldIndex = column.OrderIndex;
        int newIndex = reorderColumnDto.OrderIndex;

        if (oldIndex == newIndex)
        {
            return new ColumnDto(column.Id, column.Name, column.OrderIndex, column.CreatedAt, column.UpdatedAt);
        }

        await using IDbContextTransaction transaction = await mainDbContext.Database.BeginTransactionAsync(ct);

        try
        {
            if (oldIndex < newIndex)
            {
                logger.LogColumnShiftApplied(column.BoardId, oldIndex + 1, newIndex);

                await mainDbContext.Columns
                    .Where(x => x.BoardId == column.BoardId && x.OrderIndex > oldIndex && x.OrderIndex <= newIndex)
                    .ExecuteUpdateAsync(setter => setter.SetProperty(x => x.OrderIndex, x => x.OrderIndex - 1), ct);
            }
            else
            {
                logger.LogColumnShiftApplied(column.BoardId, newIndex, oldIndex - 1);

                await mainDbContext.Columns
                    .Where(x => x.BoardId == column.BoardId && x.OrderIndex < oldIndex && x.OrderIndex >= newIndex)
                    .ExecuteUpdateAsync(setter => setter.SetProperty(x => x.OrderIndex, x => x.OrderIndex + 1), ct);
            }

            column.OrderIndex = newIndex;
            column.UpdatedAt = DateTime.UtcNow;

            await mainDbContext.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            logger.LogColumnReordered(column.Id, oldIndex, newIndex);

            return new ColumnDto(column.Id, column.Name, column.OrderIndex, column.CreatedAt, column.UpdatedAt);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(ct);
            logger.LogColumnReorderError(reorderColumnDto.Id, ex.Message);

            return Result<ColumnDto>.Failure("An error occurred during update.", ErrorType.Unexpected);
        }
    }
}
