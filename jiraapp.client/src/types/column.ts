import type Task from "./task";

export default interface Column {
    id: string;
    name: string;
    orderIndex: number;
    createdAt: Date;
    updatedAt: Date;
    tasks: Task[];
}