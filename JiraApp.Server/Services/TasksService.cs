namespace JiraApp.Server.Services;

public class TasksService(
    MainDbContext mainDbContext,
    ILogger<TasksService> logger) : ITasksService
{
    public async Task<Result<TaskDto>> CreateTaskAsync(Guid columnId, CreateTaskDto createTaskDto, CancellationToken ct)
    {
        logger.LogTaskCreationStarted(createTaskDto.Title, columnId);

        bool columnExists = await mainDbContext.Columns.AnyAsync(x => x.Id == columnId, ct);
        if (!columnExists)
        {
            logger.LogColumnNotFoundForTask(columnId);
            return Result<TaskDto>.Failure($"Column '{columnId}' does not exist.", ErrorType.NotFound);
        }

        using IDbContextTransaction transaction = await mainDbContext.Database.BeginTransactionAsync(ct);

        try
        {
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

            await transaction.CommitAsync(ct);

            return new TaskDto(task.Id, task.Title, task.Description, task.OrderIndex, Convert.ToBase64String(task.Version));
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(ct);
            logger.LogTaskCreationError(createTaskDto.Title, ex.Message);

            return Result<TaskDto>.Failure("An error occurred during creation.", ErrorType.Unexpected);
        }
    }

    public async Task<BaseResult> DeleteTaskAsync(Guid id, CancellationToken ct)
    {
        var taskInfo = await mainDbContext.Tasks
                .Where(x => x.Id == id)
                .Select(x => new { x.ColumnId, x.OrderIndex })
                .FirstOrDefaultAsync(ct);

        if (taskInfo is null)
        {
            logger.LogTaskNotFound(id);
            return BaseResult.Failure($"Task '{id}' does not exist.", ErrorType.NotFound);
        }

        using IDbContextTransaction transaction = await mainDbContext.Database.BeginTransactionAsync(ct);

        try
        {
            await mainDbContext.Tasks
                .Where(x => x.Id == id)
                .ExecuteDeleteAsync(ct);

            // Shift the indices of all subsequent boards down by 1
            await mainDbContext.Tasks
                .Where(x => x.ColumnId == taskInfo.ColumnId && x.OrderIndex > taskInfo.OrderIndex)
                .ExecuteUpdateAsync(setter => setter.SetProperty(
                    x => x.OrderIndex,
                    x => x.OrderIndex - 1),
                ct);

            await transaction.CommitAsync(ct);
            logger.LogTaskDeleted(id, taskInfo?.ColumnId ?? Guid.Empty);

            return BaseResult.Success();
        }
        catch (DbUpdateConcurrencyException)
        {
            await transaction.RollbackAsync(ct);
            logger.LogTaskConcurrencyConflict(id);

            return Result<TaskDto>.Failure($"Someone else was modifying this record at the same time.", ErrorType.Concurrency);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(ct);
            logger.LogTaskDeleteError(id, ex.Message);

            return Result<TaskDto>.Failure("An error occurred during deletion.", ErrorType.Unexpected);
        }
    }

    public async Task<Result<TaskDto>> MoveTaskAsync(MoveTaskDto moveTaskDto, CancellationToken ct)
    {
        bool columnExists = await mainDbContext.Columns.AnyAsync(x => x.Id == moveTaskDto.ColumnId, ct);
        if (!columnExists)
        {
            return Result<TaskDto>.Failure($"Column '{moveTaskDto.ColumnId}' does not exist.", ErrorType.NotFound);
        }

        TaskModel? task = await mainDbContext.Tasks.FirstOrDefaultAsync(x => x.Id == moveTaskDto.Id, ct);
        if (task is null)
        {
            logger.LogTaskNotFound(moveTaskDto.Id);
            return Result<TaskDto>.Failure($"Task '{moveTaskDto.Id}' does not exist.", ErrorType.NotFound);
        }

        mainDbContext.Entry(task).Property(nameof(TaskModel.Version)).OriginalValue = Convert.FromBase64String(moveTaskDto.Version);
        using IDbContextTransaction transaction = await mainDbContext.Database.BeginTransactionAsync(ct);

        try
        {
            Guid originalColumnId = task.ColumnId;

            logger.LogTaskShiftApplied(task.ColumnId, task.OrderIndex + 1, int.MaxValue, "Down");
            await mainDbContext.Tasks
                .Where(x => x.ColumnId == task.ColumnId && x.OrderIndex > task.OrderIndex)
                .ExecuteUpdateAsync(setter => setter.SetProperty(
                    x => x.OrderIndex,
                    x => x.OrderIndex - 1),
                ct);

            logger.LogTaskShiftApplied(task.ColumnId, moveTaskDto.OrderIndex, int.MaxValue, "Up");
            await mainDbContext.Tasks
                    .Where(x => x.ColumnId == moveTaskDto.ColumnId && x.OrderIndex >= moveTaskDto.OrderIndex)
                    .ExecuteUpdateAsync(setter => setter.SetProperty(x => x.OrderIndex, x => x.OrderIndex + 1), ct);

            task.ColumnId = moveTaskDto.ColumnId;
            task.OrderIndex = moveTaskDto.OrderIndex;

            await mainDbContext.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            logger.LogTaskMovedAcrossColumns(moveTaskDto.Id, originalColumnId, moveTaskDto.ColumnId, moveTaskDto.OrderIndex);

            return new TaskDto(task.Id, task.Title, task.Description ?? string.Empty, task.OrderIndex, Convert.ToBase64String(task.Version));
        }
        catch (DbUpdateConcurrencyException)
        {
            await transaction.RollbackAsync(ct);
            logger.LogTaskConcurrencyConflict(moveTaskDto.Id);

            return Result<TaskDto>.Failure($"Someone else was modifying this record at the same time.", ErrorType.Concurrency);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(ct);
            logger.LogTaskMoveError(moveTaskDto.Id, ex.Message);

            return Result<TaskDto>.Failure("An error occurred during move.", ErrorType.Unexpected);
        }
    }

    public async Task<Result<TaskDto>> ReorderTaskAsync(ReorderTaskDto reorderTaskDto, CancellationToken ct)
    {
        TaskModel? task = await mainDbContext.Tasks.FirstOrDefaultAsync(x => x.Id == reorderTaskDto.Id, ct);
        if (task is null)
        {
            logger.LogTaskNotFound(reorderTaskDto.Id);
            return Result<TaskDto>.Failure($"Task '{reorderTaskDto.Id}' does not exist.", ErrorType.NotFound);
        }

        int oldIndex = task.OrderIndex;
        int newIndex = reorderTaskDto.OrderIndex;

        if (oldIndex == newIndex)
        {
            return new TaskDto(task.Id, task.Title, task.Description ?? string.Empty, task.OrderIndex, Convert.ToBase64String(task.Version));
        }

        mainDbContext.Entry(task).Property(nameof(TaskModel.Version)).OriginalValue = Convert.FromBase64String(reorderTaskDto.Version);
        using IDbContextTransaction transaction = await mainDbContext.Database.BeginTransactionAsync(ct);

        try
        {
            if (oldIndex < newIndex)
            {
                logger.LogTaskShiftApplied(task.ColumnId, oldIndex + 1, newIndex, "Down");

                await mainDbContext.Tasks
                    .Where(x => x.ColumnId == task.ColumnId && x.OrderIndex > oldIndex && x.OrderIndex <= newIndex)
                    .ExecuteUpdateAsync(setter => setter.SetProperty(x => x.OrderIndex, x => x.OrderIndex - 1), ct);
            }
            else
            {
                logger.LogTaskShiftApplied(task.ColumnId, newIndex, oldIndex - 1, "Up");

                await mainDbContext.Tasks
                    .Where(x => x.ColumnId == task.ColumnId && x.OrderIndex < oldIndex && x.OrderIndex >= newIndex)
                    .ExecuteUpdateAsync(setter => setter.SetProperty(x => x.OrderIndex, x => x.OrderIndex + 1), ct);
            }

            task.OrderIndex = newIndex;
            task.UpdatedAt = DateTime.UtcNow;

            await mainDbContext.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            logger.LogTaskReordered(reorderTaskDto.Id, oldIndex, newIndex);

            return new TaskDto(task.Id, task.Title, task.Description ?? string.Empty, task.OrderIndex, Convert.ToBase64String(task.Version));
        }
        catch (DbUpdateConcurrencyException)
        {
            await transaction.RollbackAsync(ct);
            logger.LogTaskConcurrencyConflict(reorderTaskDto.Id);

            return Result<TaskDto>.Failure($"Someone else was modifying this record at the same time.", ErrorType.Concurrency);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(ct);
            logger.LogTaskMoveError(reorderTaskDto.Id, ex.Message);

            return Result<TaskDto>.Failure("An error occurred during reorder.", ErrorType.Unexpected);
        }
    }

    public async Task<Result<TaskDto>> UpdateTaskAsync(Guid id, EditTaskDto updateTaskDto, CancellationToken ct)
    {
        logger.LogTaskUpdateStarted(id, updateTaskDto.Title);

        try
        {
            TaskModel? task = await mainDbContext.Tasks.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (task is null)
            {
                logger.LogTaskNotFound(id);
                return Result<TaskDto>.Failure($"Task '{id}' does not exist.", ErrorType.NotFound);
            }

            mainDbContext.Entry(task).Property(nameof(TaskModel.Version)).OriginalValue = Convert.FromBase64String(updateTaskDto.Version);

            task.Title = updateTaskDto.Title;
            task.Description = updateTaskDto.Description;
            task.UpdatedAt = DateTime.UtcNow;

            await mainDbContext.SaveChangesAsync(ct);

            return new TaskDto(task.Id, task.Title, task.Description, task.OrderIndex, Convert.ToBase64String(task.Version));
        }
        catch (DbUpdateConcurrencyException)
        {
            logger.LogTaskConcurrencyConflict(id);

            return Result<TaskDto>.Failure($"Someone else was modifying this record at the same time.", ErrorType.Concurrency);
        }
        catch (Exception ex)
        {
            logger.LogTaskUpdateError(id, ex.Message);

            return Result<TaskDto>.Failure("An error occurred during update.", ErrorType.Unexpected);
        }
    }
}
