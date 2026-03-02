namespace JiraApp.Server.Services.Interfaces;

public interface IBoardsService
{
    Task<Result<BoardDto>> CreateBoardAsync(CreateBoardDto createBoardDto, CancellationToken ct);
    Task<BaseResult> DeleteBoardAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<BoardDto>> GetAllBoardsAsync(CancellationToken token);
    Task<Result<BoardDto>> UpdateBoardAsync(EditBoardDto editBoardDto, Guid id, CancellationToken ct);
}
