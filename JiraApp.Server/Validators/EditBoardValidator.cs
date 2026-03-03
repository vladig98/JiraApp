namespace JiraApp.Server.Validators;

public class EditBoardValidator : AbstractValidator<EditBoardDto>
{
    public EditBoardValidator(MainDbContext mainDbContext)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(3)
            .MustAsync(async (name, cancellation) =>
                !await mainDbContext.Boards.AnyAsync(b => b.Name == name, cancellation))
            .WithMessage("A board with this name already exists.");
    }
}
