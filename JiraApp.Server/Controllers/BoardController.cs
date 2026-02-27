namespace JiraApp.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BoardController(IBoardsService boardsService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetBoards(CancellationToken ct)
    {
        List<BoardDto> boards = await boardsService.GetAllBoardsAsync(ct);

        return Ok(boards);
    }
}
