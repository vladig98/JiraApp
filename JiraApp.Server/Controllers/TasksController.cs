namespace JiraApp.Server.Controllers;

[Route("tasks")]
[ApiController]
public class TasksController(ITasksService tasksService, IHubContext hubContext) : ControllerBase
{
    [HttpPost("/columns/{columnId:Guid}/tasks")]
    public async Task<IActionResult> Create(Guid columnId, CreateTaskDto createTaskDto, CancellationToken ct)
    {
        Result<TaskDto> taskResult = await tasksService.CreateTaskAsync(columnId, createTaskDto, ct);
        if (taskResult.IsFailure)
        {
            return StatusCodeBasedOnErrorType(taskResult);
        }

        await hubContext.Clients.All.SendAsync(nameof(HubEvents.ReceiveTaskCreated), ct);
        return Ok(taskResult.Data);
    }

    [HttpPut("{id:Guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateTaskDto updateTaskDto, CancellationToken ct)
    {
        Result<TaskDto> taskResult = await tasksService.UpdateTaskAsync(id, updateTaskDto, ct);
        if (taskResult.IsFailure)
        {
            return StatusCodeBasedOnErrorType(taskResult);
        }

        await hubContext.Clients.All.SendAsync(nameof(HubEvents.ReceiveTaskUpdated), ct);
        return Ok(taskResult.Data);
    }

    [HttpDelete("{id:Guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        BaseResult taskResult = await tasksService.DeleteTaskAsync(id, ct);
        if (taskResult.IsFailure)
        {
            return StatusCodeBasedOnErrorType(taskResult);
        }

        await hubContext.Clients.All.SendAsync(nameof(HubEvents.ReceiveTaskDeleted), ct);
        return NoContent();
    }

    [HttpPut("move")]
    public async Task<IActionResult> Move(MoveTaskDto moveTaskDto, CancellationToken ct)
    {
        Result<TaskDto> taskResult = await tasksService.MoveTaskAsync(moveTaskDto, ct);
        if (taskResult.IsFailure)
        {
            return StatusCodeBasedOnErrorType(taskResult);
        }

        await hubContext.Clients.All.SendAsync(nameof(HubEvents.ReceiveTaskMoved), ct);
        return Ok(taskResult.Data);
    }

    [HttpPut("reorder")]
    public async Task<IActionResult> Reorder(ReorderTaskDto reorderTaskDto, CancellationToken ct)
    {
        Result<TaskDto> taskResult = await tasksService.ReorderTaskAsync(reorderTaskDto, ct);
        if (taskResult.IsFailure)
        {
            return StatusCodeBasedOnErrorType(taskResult);
        }

        await hubContext.Clients.All.SendAsync(nameof(HubEvents.ReceiveTaskMoved), ct);
        return Ok(taskResult.Data);
    }

    private ObjectResult StatusCodeBasedOnErrorType(BaseResult boardResult)
    {
        return boardResult.ErrorType switch
        {
            ErrorType.NotFound => NotFound(boardResult.Error),
            _ or ErrorType.Unexpected => StatusCode(500, boardResult.Error),
        };
    }
}
