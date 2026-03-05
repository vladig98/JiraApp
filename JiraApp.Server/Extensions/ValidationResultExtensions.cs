namespace JiraApp.Server.Extensions;

public static class ValidationResultExtensions
{
    extension(ValidationResult validationResult)
    {
        public ErrorResponse MapValidationError()
        {
            return new ErrorResponse(
                StatusCodes.Status400BadRequest,
                nameof(HttpStatusCode.BadRequest),
                validationResult.Errors.Select(x => x.ErrorMessage));
        }
    }
}
