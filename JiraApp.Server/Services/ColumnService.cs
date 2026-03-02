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
        int currentCount = await mainDbContext.Columns.CountAsync(ct);

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
        int deletedEntries = await mainDbContext.Columns.Where(x => x.Id == id).ExecuteDeleteAsync(ct);
        if (deletedEntries == 0)
        {
            return BaseResult.Failure($"Column with id {id} does not exist.", ErrorType.NotFound);
        }

        return BaseResult.Success();
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
        ColumnModel? column = await mainDbContext.Columns.FirstOrDefaultAsync(x => x.Id == reorderColumnDto.Id, ct);
        if (column is null)
        {
            return Result<ColumnDto>.Failure($"Column with id {reorderColumnDto.Id} does not exist.", ErrorType.NotFound);
        }

        column.OrderIndex = reorderColumnDto.OrderIndex;
        column.UpdatedAt = DateTime.UtcNow;

        await mainDbContext.SaveChangesAsync(ct);

        return new ColumnDto(column.Id, column.Name, column.OrderIndex, column.CreatedAt, column.UpdatedAt);
    }
}
