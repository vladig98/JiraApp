import type ColumnProps from "../types/columnProps";
import type Task from "../types/task";
import type Board from "../types/board";
import { useBoardStore } from '../stores/useBoardStore'
import { memo } from 'react';
import TaskCard from "./task";
import { Link, useNavigate } from '@tanstack/react-router'
import { CollisionPriority } from '@dnd-kit/abstract';
import { useSortable, isSortable } from '@dnd-kit/react/sortable';
import { useDragDropMonitor } from '@dnd-kit/react';

const Column = memo(({ column, index }: ColumnProps) => {
    const columnTasks = column.tasks as Task[];
    const navigate = useNavigate();
    const upsert = useBoardStore((state) => state.upsert);
    const getTask = useBoardStore((state) => state.getTask);

    const { ref } = useSortable({
        id: column.id,
        index,
        type: 'column',
        collisionPriority: CollisionPriority.Low,
        accept: ['column']
    });

    async function refetchBoards() {
        const boardsRepsonse = await fetch('/boards')
        if (!boardsRepsonse.ok) {
            throw new Error();
        }

        const data = await boardsRepsonse.json() as Board[]
        const sorted = [...data].sort((a, b) => a.orderIndex - b.orderIndex);

        for (const b of sorted) {
            upsert(b);
        }
    }

    async function reorderColumns(columnId: string, index: number) {
        try {
            const response = await fetch("/columns/reorder", {
                method: 'PUT',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ id: columnId, orderIndex: index })
            });

            if (!response.ok) {
                throw new Error();
            }

            await refetchBoards();
        } catch (error) {
            // Trigger the drag and rop manually to revert
            console.log(error)
        }
    }

    async function reorderTasks(taskId: string, index: number, sourceColumnId: string, targetColumnId: string | null) {
        const task = getTask(taskId);
        const url = targetColumnId ? "/tasks/move" : "/tasks/reorder";
        const body = targetColumnId ? JSON.stringify({ id: taskId, columnId: targetColumnId, orderIndex: index, version: task.version })
            : JSON.stringify({ id: taskId, orderIndex: index, version: task.version });

        try {
            const response = await fetch(url, {
                method: 'PUT',
                headers: { 'Content-Type': 'application/json' },
                body: body
            });

            if (!response.ok) {
                throw new Error();
            }

            await refetchBoards();
        } catch (error) {
            // Trigger the drag and rop manually to revert
            console.log(error)
        }
    }

    useDragDropMonitor({
        onDragEnd(event, manager) {
            const { operation, canceled } = event;

            if (canceled) {
                return;
            }

            const itemId = operation.target?.id as string;
            if (!itemId) {
                return;
            }

            const { source } = event.operation;
            if (!isSortable(source)) {
                return;
            }

            const { type, index, initialGroup, group } = source;

            switch (type) {
                case "column":
                    reorderColumns(itemId, index);
                    break;
                case "item":
                    reorderTasks(itemId, index, initialGroup as string, group as string);
                    break;
                default:
                    return;
            }
        }
    });


    function goToCreateTask() {
        navigate({ to: '/columns/$columnId/tasks/create', params: { columnId: column.id } });
    }

    return (
        <div ref={ref} className={"Column w-80 flex flex-col max-h-full"}>
            {/* Column Header */}
            <div className="flex items-center justify-between mb-4 px-1">
                <div className="flex items-center gap-2">
                    <div className="w-2 h-2 rounded-full" style={{ backgroundColor: "0052CC" }} />
                    <h3 className="font-bold text-slate-600 text-sm uppercase tracking-wide">
                        {column.name}
                    </h3>
                    <span className="text-xs bg-slate-200 text-slate-600 px-2 py-0.5 rounded-full font-bold">
                        {columnTasks.length}
                    </span>
                    <div className="col-span-1 pr-6 flex justify-end gap-1">
                        <Link
                            to="/columns/update/$columnId"
                            params={{ columnId: column.id }}
                            className="p-2 text-slate-400 hover:text-[#0052CC] hover:bg-blue-50 rounded transition-colors"
                            title="Update"
                        >
                            <span className="text-lg">✎</span>
                        </Link>
                        <Link
                            to="/columns/delete/$columnId"
                            params={{ columnId: column.id }}
                            className="p-2 text-slate-400 hover:text-red-600 hover:bg-red-50 rounded transition-colors"
                            title="Delete"
                        >
                            <span className="text-lg">🗑</span>
                        </Link>
                    </div>
                </div>
            </div>

            {/* Column Body */}
            <div className="bg-slate-100/50 rounded-xl p-3 flex-1 border border-slate-200/60 overflow-y-auto space-y-3 shadow-inner custom-scrollbar">
                {columnTasks.length > 0 ? (
                    columnTasks.map((task, index) => (
                        <TaskCard key={task.id} task={task} index={index} column={column.id} />
                    ))
                ) : (
                    <div className="py-8 border-2 border-dashed border-slate-300 rounded-lg flex items-center justify-center text-slate-400 text-sm italic text-center px-4">
                        Drop tasks here
                    </div>
                )}
                <button onClick={goToCreateTask} className="w-full group border-dashed border-slate-300 rounded-xl p-4 rounded-lg border border-slate-200 shadow-sm hover:border-[#0052CC] hover:shadow-md transition-all cursor-grab active:cursor-grabbing">
                    <span className="text-xl mr-2 group-hover:scale-125 transition-transform">+</span>
                    Add Task
                </button>
            </div>
        </div>
    );
}, (prev, next) => {
    return prev.column.id === next.column.id;
});

export default Column;