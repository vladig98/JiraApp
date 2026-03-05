namespace JiraApp.Server.Dtos.Tasks;

public sealed record class EditTaskDto(string Title, string Description, string Version);
