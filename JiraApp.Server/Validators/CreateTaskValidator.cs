namespace JiraApp.Server.Validators;

public class CreateTaskValidator : AbstractValidator<CreateTaskDto>
{
    public CreateTaskValidator(MainDbContext mainDbContext)
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MinimumLength(3)
            .CustomAsync(async (title, context, ct) =>
            {
                context.RootContextData.TryGetValue(Constants.ColumnId, out object? columnIdObject);
                if (columnIdObject is null)
                {
                    context.AddFailure("Missing column Id.");
                    return;
                }

                if (columnIdObject is not Guid columnId)
                {
                    context.AddFailure("Missing a valid column Id.");
                    return;
                }

                bool exists = await mainDbContext.Tasks.AnyAsync(c => c.Title == title && c.ColumnId == columnId, ct);
                if (exists)
                {
                    context.AddFailure("A task with this name already exists.");
                }
            });

        RuleFor(x => x.Description)
            .NotEmpty()
            .MinimumLength(3);
    }
}
