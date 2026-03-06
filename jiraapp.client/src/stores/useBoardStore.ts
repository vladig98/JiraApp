import { create } from 'zustand'
import type Board from '../types/board';
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
    any: () => _internalMap && Object.keys(_internalMap).length > 0
}));