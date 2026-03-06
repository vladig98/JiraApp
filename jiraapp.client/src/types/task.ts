export default interface Task {
    id: string;
    title: string;
    description: string;
    orderIndex: number;
    createdAt: Date;
    updatedAt: Date;
    version: string;
}