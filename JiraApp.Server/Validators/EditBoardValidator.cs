namespace JiraApp.Server.Validators;

public class EditBoardValidator : AbstractValidator<EditBoardDto>
{
    public EditBoardValidator(MainDbContext mainDbContext)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(3)
            .CustomAsync(async (name, context, cancellation) =>
            {
                context.RootContextData.TryGetValue(Constants.BoardId, out object? boardIdObject);
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

                bool exists = await mainDbContext.Boards.AnyAsync(b => b.Name == name && b.Id != boardId, cancellation);
                if (exists)
                {
                    context.AddFailure("A board with this name already exists.");
                }
            });
    }
}
