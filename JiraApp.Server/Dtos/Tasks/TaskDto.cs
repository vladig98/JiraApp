namespace JiraApp.Server.Dtos.Tasks;

public readonly record struct TaskDto(Guid Id, string Title, string Description, int OrderIndex, string Version);
