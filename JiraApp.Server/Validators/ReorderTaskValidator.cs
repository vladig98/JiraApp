namespace JiraApp.Server.Validators;

public class ReorderTaskValidator : AbstractValidator<ReorderTaskDto>
{
    public ReorderTaskValidator(MainDbContext mainDbContext)
    {
        RuleFor(x => x.OrderIndex)
            .GreaterThanOrEqualTo(0)
            .MustAsync(async (dto, index, ct) =>
            {
                TaskModel? task = await mainDbContext.Tasks.FirstOrDefaultAsync(x => x.Id == dto.Id, ct);
                if (task is null)
                {
                    return false;
                }

                int count = await mainDbContext.Tasks.CountAsync(c => c.ColumnId == task.ColumnId, ct);
                return index <= count;
            })
            .WithMessage("Order index cannot exceed the total number of tasks."); ;
    }
}
