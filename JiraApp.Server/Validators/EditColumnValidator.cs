namespace JiraApp.Server.Validators;

public class EditColumnValidator : AbstractValidator<EditColumnDto>
{
    public EditColumnValidator(MainDbContext mainDbContext)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(3)
            .CustomAsync(async (name, context, ct) =>
            {
                context.RootContextData.TryGetValue("ColumnId", out object? columnIdObject);
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

                ColumnModel? column = await mainDbContext.Columns.FirstOrDefaultAsync(x => x.Id == columnId, ct);
                if (column is null)
                {
                    context.AddFailure($"Column with id {columnId} does not exist.");
                    return;
                }

                bool exists = await mainDbContext.Columns.AnyAsync(c => c.Name == name && c.BoardId == column.BoardId && c.Id != columnId, ct);
                if (exists)
                {
                    context.AddFailure("A column with this name already exists.");
                }
            });
    }
}
