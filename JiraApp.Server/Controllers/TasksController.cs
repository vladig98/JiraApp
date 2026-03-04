namespace JiraApp.Server.Controllers;

[Route("tasks")]
[ApiController]
public class TasksController(
    ITasksService tasksService,
    IHubContext hubContext,
    IValidator<CreateTaskDto> createTaskValidator,
    IValidator<EditTaskDto> editTaskValidator,
    IValidator<MoveTaskDto> moveTaskValidator,
    IValidator<ReorderTaskDto> reorderTaskValidator) : ControllerBase
{
    [HttpPost("/columns/{columnId:Guid}/tasks")]
    public async Task<IActionResult> Create(Guid columnId, CreateTaskDto createTaskDto, CancellationToken ct)
    {
        ValidationContext<CreateTaskDto> context = new(createTaskDto);
        context.RootContextData["ColumnId"] = columnId;

        ValidationResult validationResult = await createTaskValidator.ValidateAsync(context, ct);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        Result<TaskDto> taskResult = await tasksService.CreateTaskAsync(columnId, createTaskDto, ct);
        if (taskResult.IsFailure)
        {
            return StatusCodeBasedOnErrorType(taskResult);
        }

        await hubContext.Clients.All.SendAsync(nameof(HubEvents.ReceiveTaskCreated), taskResult.Data, cancellationToken: ct);
        return Ok(taskResult.Data);
    }

    [HttpPut("{id:Guid}")]
    public async Task<IActionResult> Update(Guid id, EditTaskDto editTaskDto, CancellationToken ct)
    {
        ValidationContext<EditTaskDto> context = new(editTaskDto);
        context.RootContextData["TaskId"] = id;

        ValidationResult validationResult = await editTaskValidator.ValidateAsync(context, ct);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        Result<TaskDto> taskResult = await tasksService.UpdateTaskAsync(id, editTaskDto, ct);
        if (taskResult.IsFailure)
        {
            return StatusCodeBasedOnErrorType(taskResult);
        }

        await hubContext.Clients.All.SendAsync(nameof(HubEvents.ReceiveTaskUpdated), taskResult.Data, cancellationToken: ct);
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

        await hubContext.Clients.All.SendAsync(nameof(HubEvents.ReceiveTaskDeleted), cancellationToken: ct);
        return NoContent();
    }

    [HttpPut("move")]
    public async Task<IActionResult> Move(MoveTaskDto moveTaskDto, CancellationToken ct)
    {
        ValidationResult validationResult = await moveTaskValidator.ValidateAsync(moveTaskDto, ct);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        Result<TaskDto> taskResult = await tasksService.MoveTaskAsync(moveTaskDto, ct);
        if (taskResult.IsFailure)
        {
            return StatusCodeBasedOnErrorType(taskResult);
        }

        await hubContext.Clients.All.SendAsync(nameof(HubEvents.ReceiveTaskMoved), taskResult.Data, cancellationToken: ct);
        return Ok(taskResult.Data);
    }

    [HttpPut("reorder")]
    public async Task<IActionResult> Reorder(ReorderTaskDto reorderTaskDto, CancellationToken ct)
    {
        ValidationResult validationResult = await reorderTaskValidator.ValidateAsync(reorderTaskDto, ct);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        Result<TaskDto> taskResult = await tasksService.ReorderTaskAsync(reorderTaskDto, ct);
        if (taskResult.IsFailure)
        {
            return StatusCodeBasedOnErrorType(taskResult);
        }

        await hubContext.Clients.All.SendAsync(nameof(HubEvents.ReceiveTaskMoved), taskResult.Data, cancellationToken: ct);
        return Ok(taskResult.Data);
    }

    private ObjectResult StatusCodeBasedOnErrorType(BaseResult boardResult)
    {
        return boardResult.ErrorType switch
        {
            ErrorType.NotFound => NotFound(boardResult.Error),
            ErrorType.Concurrency => Conflict(boardResult.Error),
            _ or ErrorType.Unexpected => StatusCode(500, boardResult.Error),
        };
    }
}
