namespace JiraApp.Server.Controllers;

[ApiController]
public abstract class BaseController : ControllerBase
{
    protected ObjectResult Problem(BaseResult result)
    {
        var (statusCode, name) = result.ErrorType switch
        {
            ErrorType.NotFound => (StatusCodes.Status404NotFound, nameof(HttpStatusCode.NotFound)),
            ErrorType.Concurrency => (StatusCodes.Status409Conflict, nameof(HttpStatusCode.Conflict)),
            ErrorType.Unexpected => (StatusCodes.Status500InternalServerError, nameof(HttpStatusCode.InternalServerError)),
            _ => (StatusCodes.Status500InternalServerError, "Error")
        };

        var response = new ErrorResponse(statusCode, name, [result.Error ?? "An unknown error occurred."]);
        return StatusCode(statusCode, response);
    }
}
