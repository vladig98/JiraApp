namespace JiraApp.Server.Dtos.Columns;

public readonly record struct ColumnDto(Guid Id, string Name, int OrderIndex, DateTime CreatedAt, DateTime UpdatedAt);
