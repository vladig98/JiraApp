import type Board from "./board";
import type Column from "./column";
import type Task from "./task";

export default interface BoardActions {
    init: (boards: Board[]) => void;
    upsert: (board: Board) => void;
    remove: (id: string) => void;
    getBoard: (id: string) => Board | undefined;
    getAll: () => Board[];
    any: () => boolean;

    upsertColumn: (column: Column, boardId: string | null) => void;
    removeColumn: (columnId: string) => void;
    getColumn: (columnId: string) => Column;

    upsertTask: (task: Task, columnId: string | null) => void;
    removeTask: (taskId: string) => void;
    getTask: (taskId: string) => Task;

    getBoardByColumnId: (columnId: string) => Board | undefined;
    getBoardByTaskId: (taskId: string) => Board | undefined;
    getColumnByTaskId: (taskId: string) => Column | undefined;
}