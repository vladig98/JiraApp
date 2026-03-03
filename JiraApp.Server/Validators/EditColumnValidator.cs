namespace JiraApp.Server.Validators;

public class EditColumnValidator : AbstractValidator<EditColumnDto>
{
    public EditColumnValidator(MainDbContext mainDbContext)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(3)
            .MustAsync(async (name, cancellation) =>
                !await mainDbContext.Columns.AnyAsync(b => b.Name == name, cancellation))
            .WithMessage("A column with this name already exists.");
    }
}
