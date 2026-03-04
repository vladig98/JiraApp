namespace JiraApp.Server.Controllers;

[Route("columns")]
[ApiController]
public class ColumnsController(
    IColumnService columnService,
    IHubContext hubContext,
    IValidator<CreateColumnDto> createColumnValidator,
    IValidator<EditColumnDto> editColumnValidator,
    IValidator<ReorderColumnDto> reorderColumnValidator) : ControllerBase
{
    [HttpPost("/boards/{boardId:Guid}/columns")]
    public async Task<ActionResult<ColumnDto>> Create(Guid boardId, CreateColumnDto createColumnDto, CancellationToken ct)
    {
        ValidationContext<CreateColumnDto> context = new(createColumnDto);
        context.RootContextData["BoardId"] = boardId;

        ValidationResult validationResult = await createColumnValidator.ValidateAsync(context, ct);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        Result<ColumnDto> columnResult = await columnService.CreateColumnAsync(boardId, createColumnDto, ct);
        if (columnResult.IsFailure)
        {
            return StatusCodeBasedOnErrorType(columnResult);
        }

        return Ok(columnResult.Data);
    }

    [HttpPut("{id:Guid}")]
    public async Task<IActionResult> Update(Guid id, EditColumnDto editColumnDto, CancellationToken ct)
    {
        ValidationContext<EditColumnDto> context = new(editColumnDto);
        context.RootContextData["ColumnId"] = id;

        ValidationResult validationResult = await editColumnValidator.ValidateAsync(context, ct);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        Result<ColumnDto> columnResult = await columnService.UpdateColumnAsync(id, editColumnDto, ct);
        if (columnResult.IsFailure)
        {
            return StatusCodeBasedOnErrorType(columnResult);
        }

        return Ok(columnResult.Data);
    }

    [HttpDelete("{id:Guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        BaseResult columnResult = await columnService.DeleteColumnAsync(id, ct);
        if (columnResult.IsFailure)
        {
            return StatusCodeBasedOnErrorType(columnResult);
        }

        return NoContent();
    }

    [HttpPut("reorder")]
    public async Task<IActionResult> Reorder(ReorderColumnDto reorderColumnDto, CancellationToken ct)
    {
        ValidationResult validationResult = await reorderColumnValidator.ValidateAsync(reorderColumnDto, ct);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        Result<ColumnDto> columnResult = await columnService.UpdateColumnOrderAsync(reorderColumnDto, ct);
        if (columnResult.IsFailure)
        {
            return StatusCodeBasedOnErrorType(columnResult);
        }

        await hubContext.Clients.All.SendAsync(nameof(HubEvents.ReceiveColumnReordered), columnResult.Data, cancellationToken: ct);
        return Ok(columnResult.Data);
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
