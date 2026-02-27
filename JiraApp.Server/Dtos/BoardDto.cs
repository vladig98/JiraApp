namespace JiraApp.Server.Dtos;

public readonly record struct BoardDto(Guid Id, string Name, int OrderIndex);