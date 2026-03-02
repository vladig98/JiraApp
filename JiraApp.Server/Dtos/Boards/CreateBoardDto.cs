namespace JiraApp.Server.Dtos.Boards;

public sealed record class CreateBoardDto(string Name, int OrderIndex);
