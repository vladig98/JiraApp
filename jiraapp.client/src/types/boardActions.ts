import type Board from "./board";

export default interface BoardActions {
    init: (boards: Board[]) => void;
    upsert: (board: Board) => void;
    remove: (id: string) => void;
    getBoard: (id: string) => Board | undefined;
    getAll: () => Board[];
    any: () => boolean;
}