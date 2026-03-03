namespace JiraApp.Server.Validators;

public class CreateTaskValidator : AbstractValidator<CreateTaskDto>
{
    public CreateTaskValidator(MainDbContext mainDbContext)
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MinimumLength(3)
            .MustAsync(async (title, cancellation) =>
                !await mainDbContext.Tasks.AnyAsync(b => b.Title == title, cancellation))
            .WithMessage("A board with this name already exists.");

        RuleFor(x => x.Description)
            .NotEmpty()
            .MinimumLength(3);
    }
}
