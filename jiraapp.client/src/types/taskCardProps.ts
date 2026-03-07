import type Task from "./task";

export default interface TaskCardProps {
    task: Task;
    index: number;
    column: string;
}