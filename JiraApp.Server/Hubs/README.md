# SignalR Event Contracts

This document defines the real-time event payloads sent from the Server to the Client. All events use **strongly typed payloads** via `Context.Clients.All.SendAsync()`.

## 1. Board Events (`BoardHub`)
| Method | Payload Shape | Description |
| :--- | :--- | :--- |
| `ReceiveBoardCreated` | `BoardDto` | Sent when a new board is initialized. |
| `ReceiveBoardUpdated` | `BoardDto` | Updates board name or global ordering. |
| `ReceiveBoardDeleted` | `Guid (id)` | Instructs client to remove board from view. |

## 2. Column Events (`ColumnHub`)
| Method | Payload Shape | Description |
| :--- | :--- | :--- |
| `ReceiveColumnCreated`| `ColumnDto` | New column added to the active board. |
| `ReceiveColumnMoved`  | `ColumnDto` | Contains the new `OrderIndex`. |
| `ReceiveColumnDeleted`| `Guid (id)` | Removes column and all nested tasks. |

## 3. Task Events (`TaskHub`)
| Method | Payload Shape | Description |
| :--- | :--- | :--- |
| `ReceiveTaskCreated`  | `TaskDto` | New task added to a column. |
| `ReceiveTaskMoved`    | `TaskDto` | **Crucial:** Includes `ColumnId` and `OrderIndex` for re-sorting. |
| `ReceiveTaskDeleted`  | `Guid (id)` | Removes task from the specific column. |

---

### Implementation Details (C# Interfaces)

```csharp
namespace JiraApp.Server.Hubs.Interfaces;

public interface IBoardClient
{
    /// <summary> Broadcasts a full Board object upon creation. </summary>
    Task ReceiveBoardCreated(BoardDto board);

    /// <summary> Broadcasts updated Board metadata. </summary>
    Task ReceiveBoardUpdated(BoardDto board);

    /// <summary> Notifies clients to remove a board from the UI cache by Id. </summary>
    Task ReceiveBoardDeleted(Guid id);
}

public interface IColumnClient
{
    /// <summary> Broadcasts a new column within a specific board context. </summary>
    Task ReceiveColumnCreated(ColumnDto column);

    /// <summary> Broadcasts changes to column title or metadata. </summary>
    Task ReceiveColumnUpdated(ColumnDto column);

    /// <summary> Notifies clients to remove a column from a board. </summary>
    Task ReceiveColumnDeleted(Guid id);

    /// <summary> Broadcasts new OrderIndex for reconciliation during Drag & Drop. </summary>
    Task ReceiveColumnMoved(ColumnDto column);
}

public interface ITaskClient
{
    /// <summary> Broadcasts a new task added to a column. </summary>
    Task ReceiveTaskCreated(TaskDto task);

    /// <summary> Broadcasts updates to Title or Description. </summary>
    Task ReceiveTaskUpdated(TaskDto task);

    /// <summary> Notifies clients to purge a task from the UI. </summary>
    Task ReceiveTaskDeleted(Guid id);

    /// <summary> Broadcasts updated OrderIndex and ColumnId for state reconciliation. </summary>
    Task ReceiveTaskMoved(TaskDto task);
}
```