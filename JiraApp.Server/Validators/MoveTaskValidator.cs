namespace JiraApp.Server.Validators;

public class MoveTaskValidator : AbstractValidator<MoveTaskDto>
{
    public MoveTaskValidator(MainDbContext mainDbContext)
    {
        RuleFor(x => x.OrderIndex)
            .GreaterThanOrEqualTo(0)
            .MustAsync(async (dto, index, ct) =>
            {
                int count = await mainDbContext.Tasks.CountAsync(c => c.ColumnId == dto.ColumnId, ct);
                return index <= count;
            })
            .WithMessage("Order index cannot exceed the total number of tasks."); ;
    }
}
