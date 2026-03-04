namespace JiraApp.Server.Validators;

public class ReorderColumnValidator : AbstractValidator<ReorderColumnDto>
{
    public ReorderColumnValidator(MainDbContext mainDbContext)
    {
        RuleFor(x => x.OrderIndex)
            .GreaterThanOrEqualTo(0)
            .MustAsync(async (dto, orderIndex, cancellation) =>
            {
                ColumnModel? column = await mainDbContext.Columns.FirstOrDefaultAsync(x => x.Id == dto.Id, cancellation);
                if (column is null)
                {
                    return false;
                }

                int count = await mainDbContext.Columns.Where(x => x.BoardId == column.BoardId).CountAsync(cancellation);
                return orderIndex <= count;
            })
            .WithMessage("Order index cannot exceed the total number of columns.");
    }
}
