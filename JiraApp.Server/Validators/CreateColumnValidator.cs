namespace JiraApp.Server.Validators;

public class CreateColumnValidator : AbstractValidator<CreateColumnDto>
{
    public CreateColumnValidator(MainDbContext mainDbContext)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(3)
            .CustomAsync(async (name, context, ct) =>
            {
                context.RootContextData.TryGetValue("BoardId", out object? boardIdObject);
                if (boardIdObject is null)
                {
                    context.AddFailure("Missing board Id.");
                    return;
                }

                if (boardIdObject is not Guid boardId)
                {
                    context.AddFailure("Missing a valid board Id.");
                    return;
                }

                bool exists = await mainDbContext.Columns.AnyAsync(c => c.Name == name && c.BoardId == boardId, ct);
                if (exists)
                {
                    context.AddFailure("A column with this name already exists.");
                }
            });
    }
}
