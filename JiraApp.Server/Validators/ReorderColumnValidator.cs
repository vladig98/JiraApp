namespace JiraApp.Server.Validators;

public class ReorderColumnValidator : AbstractValidator<ReorderColumnDto>
{
    public ReorderColumnValidator(MainDbContext mainDbContext)
    {
        RuleFor(x => x.OrderIndex)
            .GreaterThanOrEqualTo(0)
            .MustAsync(async (orderIndex, cancellation) =>
            {
                int count = await mainDbContext.Columns.CountAsync(cancellation);
                return orderIndex <= count;
            })
            .WithMessage("Order index cannot exceed the total number of columns.");
    }
}
