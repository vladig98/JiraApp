import type Column from "./column";

export default interface Board {
    id: string;
    name: string,
    orderIndex: number;
    createdAt: Date;
    updatedAt: Date;
    columns: Column[];
}