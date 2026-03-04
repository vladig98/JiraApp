namespace JiraApp.Server.Controllers;

[Route("boards")]
[ApiController]
public class BoardsController(
    IBoardsService boardsService,
    IHubContext<BoardHub, IBoardClient> hubContext,
    IValidator<CreateBoardDto> createdBoardValidator,
    IValidator<EditBoardDto> editBoardValidator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<BoardDto>>> Retrieve(CancellationToken ct)
    {
        IReadOnlyList<BoardDto> boards = await boardsService.GetAllBoardsAsync(ct);
        return Ok(boards);
    }

    [HttpPost]
    public async Task<ActionResult<BoardDto>> Create(CreateBoardDto createBoardDto, CancellationToken ct)
    {
        ValidationResult validationResult = await createdBoardValidator.ValidateAsync(createBoardDto, ct);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        Result<BoardDto> boardResult = await boardsService.CreateBoardAsync(createBoardDto, ct);
        if (boardResult.IsFailure)
        {
            return StatusCodeBasedOnErrorType(boardResult);
        }

        await hubContext.Clients.All.ReceiveBoardCreated(boardResult.Data);
        return Ok(boardResult.Data);
    }

    [HttpPut("{id:Guid}")]
    public async Task<ActionResult<BoardDto>> Update(EditBoardDto editBoardDto, Guid id, CancellationToken ct)
    {
        ValidationResult validationResult = await editBoardValidator.ValidateAsync(editBoardDto, ct);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        Result<BoardDto> boardResult = await boardsService.UpdateBoardAsync(editBoardDto, id, ct);
        if (boardResult.IsFailure)
        {
            return StatusCodeBasedOnErrorType(boardResult);
        }

        await hubContext.Clients.All.ReceiveBoardUpdated(boardResult.Data);
        return Ok(boardResult.Data);
    }

    [HttpDelete("{id:Guid}")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken ct)
    {
        BaseResult boardResult = await boardsService.DeleteBoardAsync(id, ct);
        if (boardResult.IsFailure)
        {
            return StatusCodeBasedOnErrorType(boardResult);
        }

        await hubContext.Clients.All.ReceiveBoardDeleted(id);
        return NoContent();
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
