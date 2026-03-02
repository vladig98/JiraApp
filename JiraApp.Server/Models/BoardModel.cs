namespace JiraApp.Server.Models;

public class BoardModel : BaseModel
{
    public required string Name { get; set; }

    public ICollection<ColumnModel> Columns { get; set; } = [];
}
