namespace JiraApp.Server.Controllers;

[Route("columns")]
[ApiController]
public class ColumnsController : ControllerBase
{
    [HttpPost("/boards/{boardId:Guid}/columns")]
    public async Task<IActionResult> Create(Guid boardId, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    [HttpPut("{id:Guid}")]
    public async Task<IActionResult> Update(Guid id, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("{id:Guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    [HttpPut("reorder")]
    public async Task<IActionResult> Reorder(Guid id, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
