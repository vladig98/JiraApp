namespace JiraApp.Server.Validators;

public class EditTaskValidator : AbstractValidator<EditTaskDto>
{
    public EditTaskValidator(MainDbContext mainDbContext)
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MinimumLength(3)
            .CustomAsync(async (title, context, ct) =>
            {
                context.RootContextData.TryGetValue(Constants.TaskId, out object? taskIdObject);
                if (taskIdObject is null)
                {
                    context.AddFailure("Missing task Id.");
                    return;
                }

                if (taskIdObject is not Guid taskId)
                {
                    context.AddFailure("Missing a valid task Id.");
                    return;
                }

                TaskModel? task = await mainDbContext.Tasks.FirstOrDefaultAsync(x => x.Id == taskId, ct);
                if (task is null)
                {
                    context.AddFailure($"Task with id {taskId} does not exist.");
                    return;
                }

                bool exists = await mainDbContext.Tasks.AnyAsync(c => c.Title == title && c.ColumnId == task.ColumnId && c.Id != taskId, ct);
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
