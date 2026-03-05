namespace JiraApp.Server.Dtos.Tasks;

public sealed record class MoveTaskDto(Guid Id, Guid ColumnId, int OrderIndex, string Version);
