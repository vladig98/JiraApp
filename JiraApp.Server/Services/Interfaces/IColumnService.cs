using JiraApp.Server.Dtos.Columns;

namespace JiraApp.Server.Services.Interfaces;

public interface IColumnService
{
    Task<Result<ColumnDto>> CreateColumnAsync(Guid boardId, CreateColumnDto createColumnDto, CancellationToken ct);
    Task<BaseResult> DeleteColumnAsync(Guid id, CancellationToken ct);
    Task<Result<ColumnDto>> UpdateColumnAsync(Guid id, EditColumnDto editColumnDto, CancellationToken ct);
    Task<Result<ColumnDto>> UpdateColumnOrderAsync(ReorderColumnDto reorderColumnDto, CancellationToken ct);
}
