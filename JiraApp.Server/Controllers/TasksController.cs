namespace JiraApp.Server.Controllers;

[Route("tasks")]
[ApiController]
public class TasksController : ControllerBase
{
    [HttpPost("/columns/{columnId:Guid}/tasks")]
    public async Task<IActionResult> Create(Guid columnId, CancellationToken ct)
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

    [HttpPut("move")]
    public async Task<IActionResult> Move(Guid id, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    [HttpPut("reorder")]
    public async Task<IActionResult> Reorder(Guid id, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
