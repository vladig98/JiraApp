import { create } from 'zustand'
import type Board from '../types/board';
import type Column from '../types/column';
import type Task from '../types/task';
import type BoardActions from '../types/boardActions';

let _internalMap: Record<string, Board> = {};

export const useBoardStore = create<BoardActions>((set) => ({
    init: (boardArray) => {
        _internalMap = Object.fromEntries(boardArray.map(b => [b.id, b]));
        set({});
    },

    upsert: (board: Board) => {
        _internalMap = { ..._internalMap, [board.id]: board };
        set({});
    },

    remove: (id) => {
        const { [id]: _, ...remaining } = _internalMap;
        _internalMap = remaining;
        set({});
    },

    getBoard: (id) => _internalMap[id],
    getAll: () => Object.values(_internalMap),
    any: () => _internalMap && Object.keys(_internalMap).length > 0,

    upsertColumn: (column, boardId) => {
        const bid = boardId || Object.keys(_internalMap).find(id =>
            _internalMap[id].columns.some(c => c.id === column.id)
        );

        if (bid && _internalMap[bid]) {
            const board = _internalMap[bid];
            const exists = board.columns.some(c => c.id === column.id);

            _internalMap[bid] = {
                ...board,
                columns: exists
                    ? board.columns.map(c => c.id === column.id ? column : c)
                    : [...board.columns, column]
            };
            set({});
        }
    },
    removeColumn: (columnId) => {
        for (const bid in _internalMap) {
            _internalMap[bid].columns = _internalMap[bid].columns.filter(c => c.id !== columnId);
        }
        set({});
    },
    getColumn: (columnId) => {
        return Object.values(_internalMap)
            .flatMap(b => b.columns)
            .find(c => c.id === columnId) as Column;
    },

    upsertTask: (task, columnId) => {
        let taskFound = false;

        for (const bid in _internalMap) {
            const board = _internalMap[bid];
            const targetColId = columnId || board.columns.find(c =>
                c.tasks.some(t => t.id === task.id)
            )?.id;

            if (!targetColId) continue;

            const newColumns = board.columns.map(col => {
                if (col.id !== targetColId) return col;

                taskFound = true;
                const exists = col.tasks.some(t => t.id === task.id);

                return {
                    ...col,
                    tasks: exists
                        ? col.tasks.map(t => t.id === task.id ? task : t)
                        : [...col.tasks, task]
                };
            });

            if (taskFound) {
                _internalMap[bid] = { ...board, columns: newColumns };
                set({});
                break;
            }
        }
    },
    removeTask: (taskId) => {
        for (const bid in _internalMap) {
            const board = _internalMap[bid];
            let taskFound = false;

            const newColumns = board.columns.map(col => {
                const taskExists = col.tasks.some(t => t.id === taskId);
                if (taskExists) {
                    taskFound = true;
                    return {
                        ...col,
                        tasks: col.tasks.filter(t => t.id !== taskId)
                    };
                }
                return col;
            });

            if (taskFound) {
                _internalMap[bid] = { ...board, columns: newColumns };
                set({});
                break;
            }
        }
    },
    getTask: (taskId) => {
        return Object.values(_internalMap)
            .flatMap(b => b.columns)
            .flatMap(c => c.tasks)
            .find(t => t.id === taskId) as Task;
    },

    getBoardByColumnId: (columnId) => {
        for (const bid in _internalMap) {
            const board = _internalMap[bid];
            if (board.columns.some(c => c.id === columnId)) {
                return board as Board;
            }
        }
        return undefined;
    },

    getBoardByTaskId: (taskId) => {
        for (const bid in _internalMap) {
            const board = _internalMap[bid];
            const hasTask = board.columns.some(col =>
                col.tasks.some(t => t.id === taskId)
            );

            if (hasTask) {
                return board as Board;
            }
        }
        return undefined;
    },

    getColumnByTaskId: (taskId) => {
        for (const bid in _internalMap) {
            const board = _internalMap[bid];

            const targetColumn = board.columns.find(col =>
                col.tasks.some(t => t.id === taskId)
            );

            if (targetColumn) {
                return targetColumn as Column;
            }
        }
        return undefined;
    }
}));