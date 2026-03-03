namespace JiraApp.Server.Services;

public class ColumnService(MainDbContext mainDbContext) : IColumnService
{
    public async Task<Result<ColumnDto>> CreateColumnAsync(Guid boardId, CreateColumnDto createColumnDto, CancellationToken ct)
    {
        bool boardExists = await mainDbContext.Boards.AnyAsync(x => x.Id == boardId, ct);
        if (!boardExists)
        {
            return Result<ColumnDto>.Failure($"Board with id {boardId} does not exist.", ErrorType.NotFound);
        }

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

        return new ColumnDto(column.Id, column.Name, column.OrderIndex, column.CreatedAt, column.UpdatedAt);
    }

    public async Task<BaseResult> DeleteColumnAsync(Guid id, CancellationToken ct)
    {
        await using IDbContextTransaction transaction = await mainDbContext.Database.BeginTransactionAsync(ct);

        try
        {
            ColumnModel? column = await mainDbContext.Columns.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (column is null)
            {
                return BaseResult.Failure($"Column with id {id} does not exist.", ErrorType.NotFound);
            }

            mainDbContext.Columns.Remove(column);
            await mainDbContext.SaveChangesAsync(ct);

            List<ColumnModel> columns = await mainDbContext.Columns
                .Where(x => x.BoardId == column.BoardId)
                .OrderBy(x => x.OrderIndex)
                .ToListAsync(ct);

            int orderIndex = 0;
            foreach (ColumnModel dbColumn in columns)
            {
                dbColumn.OrderIndex = orderIndex++;
            }

            await mainDbContext.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            return BaseResult.Success();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }

    public async Task<Result<ColumnDto>> UpdateColumnAsync(Guid id, EditColumnDto editColumnDto, CancellationToken ct)
    {
        ColumnModel? column = await mainDbContext.Columns.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (column is null)
        {
            return Result<ColumnDto>.Failure($"Column with id {id} does not exist.", ErrorType.NotFound);
        }

        column.Name = editColumnDto.Name;
        column.UpdatedAt = DateTime.UtcNow;

        await mainDbContext.SaveChangesAsync(ct);

        return new ColumnDto(column.Id, column.Name, column.OrderIndex, column.CreatedAt, column.UpdatedAt);
    }

    public async Task<Result<ColumnDto>> UpdateColumnOrderAsync(ReorderColumnDto reorderColumnDto, CancellationToken ct)
    {
        await using IDbContextTransaction transaction = await mainDbContext.Database.BeginTransactionAsync(ct);

        try
        {
            ColumnModel? column = await mainDbContext.Columns.FirstOrDefaultAsync(x => x.Id == reorderColumnDto.Id, ct);
            if (column is null)
            {
                return Result<ColumnDto>.Failure($"Column with id {reorderColumnDto.Id} does not exist.", ErrorType.NotFound);
            }

            column.OrderIndex = -1;
            column.UpdatedAt = DateTime.UtcNow;

            await mainDbContext.SaveChangesAsync(ct);

            List<ColumnModel> columns = await mainDbContext.Columns
                .Where(x => x.BoardId == column.BoardId && x.OrderIndex >= 0)
                .OrderBy(x => x.OrderIndex)
                .ToListAsync(ct);

            int orderIndex = 0;
            foreach (ColumnModel dbColumn in columns)
            {
                if (orderIndex == reorderColumnDto.OrderIndex)
                {
                    orderIndex++;
                }

                dbColumn.OrderIndex = orderIndex++;
            }

            await mainDbContext.SaveChangesAsync(ct);

            column.OrderIndex = reorderColumnDto.OrderIndex;
            await mainDbContext.SaveChangesAsync(ct);

            await transaction.CommitAsync(ct);

            return new ColumnDto(column.Id, column.Name, column.OrderIndex, column.CreatedAt, column.UpdatedAt);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }
}
