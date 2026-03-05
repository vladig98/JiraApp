namespace JiraApp.Server.Models;

public class TaskModel : BaseModel
{
    public Guid ColumnId { get; set; }
    public ColumnModel Column { get; set; } = null!;

    public required string Title { get; set; }
    public string? Description { get; set; }

    public byte[] Version { get; set; } = null!;

    public TaskDto ToDto()
        => new(Id, Title, Description ?? string.Empty, OrderIndex, Convert.ToBase64String(Version));
}
