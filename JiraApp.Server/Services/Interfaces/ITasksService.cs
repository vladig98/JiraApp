namespace JiraApp.Server.Services.Interfaces;

public interface ITasksService
{
    Task<Result<TaskDto>> CreateTaskAsync(Guid columnId, CreateTaskDto createTaskDto, CancellationToken ct);
    Task<BaseResult> DeleteTaskAsync(Guid id, CancellationToken ct);
    Task<Result<TaskDto>> MoveTaskAsync(MoveTaskDto moveTaskDto, CancellationToken ct);
    Task<Result<TaskDto>> ReorderTaskAsync(ReorderTaskDto reorderTaskDto, CancellationToken ct);
    Task<Result<TaskDto>> UpdateTaskAsync(Guid id, EditTaskDto updateTaskDto, CancellationToken ct);
}
