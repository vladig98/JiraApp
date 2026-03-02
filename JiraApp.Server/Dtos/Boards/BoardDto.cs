namespace JiraApp.Server.Dtos.Boards;

public readonly record struct BoardDto(Guid Id, string Name, int OrderIndex, DateTime CreatedAt, DateTime UpdatedAt);