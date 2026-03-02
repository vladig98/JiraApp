namespace JiraApp.Server.Controllers;

[Route("columns")]
[ApiController]
public class ColumnsController(IColumnService columnService) : ControllerBase
{
    [HttpPost("/boards/{boardId:Guid}/columns")]
    public async Task<ActionResult<ColumnDto>> Create(Guid boardId, CreateColumnDto createColumnDto, CancellationToken ct)
    {
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
        Result<ColumnDto> columnResult = await columnService.UpdateColumnOrderAsync(reorderColumnDto, ct);
        if (columnResult.IsFailure)
        {
            return StatusCodeBasedOnErrorType(columnResult);
        }

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
