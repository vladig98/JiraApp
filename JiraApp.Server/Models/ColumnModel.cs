namespace JiraApp.Server.Models;

public class ColumnModel : BaseModel
{
    public Guid BoardId { get; set; }
    public BoardModel Board { get; set; } = null!;

    public required string Name { get; set; }
}
