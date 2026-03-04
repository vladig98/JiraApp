namespace JiraApp.Server.Services;

public class TasksService(MainDbContext mainDbContext) : ITasksService
{
    public async Task<Result<TaskDto>> CreateTaskAsync(Guid columnId, CreateTaskDto createTaskDto, CancellationToken ct)
    {
        bool columnExists = await mainDbContext.Columns.AnyAsync(x => x.Id == columnId, ct);
        if (!columnExists)
        {
            return Result<TaskDto>.Failure($"Column with id {columnId} does not exist.", ErrorType.NotFound);
        }

        DateTime now = DateTime.UtcNow;
        int currentCount = await mainDbContext.Tasks.Where(x => x.ColumnId == columnId).CountAsync(ct);

        TaskModel task = new()
        {
            ColumnId = columnId,
            CreatedAt = now,
            UpdatedAt = now,
            Title = createTaskDto.Title,
            OrderIndex = currentCount,
            Description = createTaskDto.Description
        };

        await mainDbContext.Tasks.AddAsync(task, ct);
        await mainDbContext.SaveChangesAsync(ct);

        return new TaskDto(task.Id, task.Title, task.Description, task.OrderIndex);
    }

    public async Task<BaseResult> DeleteTaskAsync(Guid id, CancellationToken ct)
    {
        using IDbContextTransaction transaction = await mainDbContext.Database.BeginTransactionAsync(ct);

        try
        {
            TaskModel? task = await mainDbContext.Tasks.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (task is null)
            {
                return BaseResult.Failure($"Task with id {id} does not exist.", ErrorType.NotFound);
            }

            mainDbContext.Tasks.Remove(task);
            await mainDbContext.SaveChangesAsync(ct);

            await UpdateOrderIndexToAllTasks(task.ColumnId, ct);
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

    public async Task<Result<TaskDto>> MoveTaskAsync(MoveTaskDto moveTaskDto, CancellationToken ct)
    {
        using IDbContextTransaction transaction = await mainDbContext.Database.BeginTransactionAsync(ct);

        try
        {
            ColumnModel? column = await mainDbContext.Columns.FirstOrDefaultAsync(x => x.Id == moveTaskDto.ColumnId, ct);
            if (column is null)
            {
                return Result<TaskDto>.Failure($"Column with id {moveTaskDto.ColumnId} does not exist.", ErrorType.NotFound);
            }

            TaskModel? task = await mainDbContext.Tasks.FirstOrDefaultAsync(x => x.Id == moveTaskDto.Id, ct);
            if (task is null)
            {
                return Result<TaskDto>.Failure($"Task with id {moveTaskDto.Id} does not exist.", ErrorType.NotFound);
            }

            Guid originalColumnId = task.ColumnId;

            task.ColumnId = moveTaskDto.ColumnId;
            task.OrderIndex = -1;
            task.UpdatedAt = DateTime.UtcNow;

            await mainDbContext.SaveChangesAsync(ct);

            await UpdateOrderIndexToAllTasks(originalColumnId, ct);
            await mainDbContext.SaveChangesAsync(ct);

            await UpdateOrderIndexToAllTasksWithId(task.ColumnId, moveTaskDto.OrderIndex, ct);
            task.OrderIndex = moveTaskDto.OrderIndex;

            await mainDbContext.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            return new TaskDto(task.Id, task.Title, task.Description ?? string.Empty, task.OrderIndex);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }

    public async Task<Result<TaskDto>> ReorderTaskAsync(ReorderTaskDto reorderTaskDto, CancellationToken ct)
    {
        using IDbContextTransaction transaction = await mainDbContext.Database.BeginTransactionAsync(ct);

        try
        {
            TaskModel? task = await mainDbContext.Tasks.FirstOrDefaultAsync(x => x.Id == reorderTaskDto.Id, ct);
            if (task is null)
            {
                return Result<TaskDto>.Failure($"Task with id {reorderTaskDto.Id} does not exist.", ErrorType.NotFound);
            }

            task.OrderIndex = -1;
            task.UpdatedAt = DateTime.UtcNow;

            await mainDbContext.SaveChangesAsync(ct);
            await UpdateOrderIndexToAllTasksWithId(task.ColumnId, reorderTaskDto.OrderIndex, ct);

            task.OrderIndex = reorderTaskDto.OrderIndex;
            await mainDbContext.SaveChangesAsync(ct);

            await transaction.CommitAsync(ct);

            return new TaskDto(task.Id, task.Title, task.Description ?? string.Empty, task.OrderIndex);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }

    public async Task<Result<TaskDto>> UpdateTaskAsync(Guid id, EditTaskDto updateTaskDto, CancellationToken ct)
    {
        TaskModel? task = await mainDbContext.Tasks.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (task is null)
        {
            return Result<TaskDto>.Failure($"Task with id {id} does not exist.", ErrorType.NotFound);
        }

        task.Title = updateTaskDto.Title;
        task.Description = updateTaskDto.Description;
        task.UpdatedAt = DateTime.UtcNow;

        await mainDbContext.SaveChangesAsync(ct);

        return new TaskDto(task.Id, task.Title, task.Description, task.OrderIndex);
    }

    private async Task UpdateOrderIndexToAllTasks(Guid columnId, CancellationToken ct)
    {
        List<TaskModel> tasks = await mainDbContext.Tasks
            .Where(x => x.ColumnId == columnId)
            .OrderBy(x => x.OrderIndex)
            .ToListAsync(ct);

        int orderIndex = 0;
        foreach (TaskModel task in tasks)
        {
            task.OrderIndex = orderIndex++;
        }
    }

    private async Task UpdateOrderIndexToAllTasksWithId(Guid columnId, int orderIndex, CancellationToken ct)
    {
        List<TaskModel> tasks = await mainDbContext.Tasks
            .Where(x => x.ColumnId == columnId && x.OrderIndex >= 0)
            .OrderBy(x => x.OrderIndex)
            .ToListAsync(ct);

        int newOrderIndex = 0;
        foreach (TaskModel task in tasks)
        {
            if (newOrderIndex == orderIndex)
            {
                newOrderIndex++;
            }

            task.OrderIndex = newOrderIndex++;
        }

        await mainDbContext.SaveChangesAsync(ct);
    }
}
