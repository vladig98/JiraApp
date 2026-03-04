namespace JiraApp.Server.Controllers;

[Route("boards")]
[ApiController]
public class BoardsController(
    IBoardsService boardsService,
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
        if (validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        Result<BoardDto> boardResult = await boardsService.CreateBoardAsync(createBoardDto, ct);
        if (boardResult.IsFailure)
        {
            return StatusCodeBasedOnErrorType(boardResult);
        }

        return CreatedAtAction(nameof(Retrieve), new { id = boardResult.Data.Id }, boardResult.Data);
    }

    [HttpPut("{id:Guid}")]
    public async Task<ActionResult<BoardDto>> Update(EditBoardDto editBoardDto, Guid id, CancellationToken ct)
    {
        ValidationResult validationResult = await editBoardValidator.ValidateAsync(editBoardDto, ct);
        if (validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        Result<BoardDto> boardResult = await boardsService.UpdateBoardAsync(editBoardDto, id, ct);
        if (boardResult.IsFailure)
        {
            return StatusCodeBasedOnErrorType(boardResult);
        }

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
