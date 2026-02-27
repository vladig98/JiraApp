namespace JiraApp.Server.Models;

public abstract class BaseModel
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int OrderIndex { get; set; }
}
