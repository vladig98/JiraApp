import type ColumnProps from "../types/columnProps";
import type Task from "../types/task";
import { memo } from 'react';
import TaskCard from "./task";

const Column = memo(({ column }: ColumnProps) => {
    const columnTasks = column.tasks as Task[];

    return (
        <div className="w-80 flex flex-col max-h-full">
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
                </div>
            </div>

            {/* Column Body */}
            <div className="bg-slate-100/50 rounded-xl p-3 flex-1 border border-slate-200/60 overflow-y-auto space-y-3 shadow-inner custom-scrollbar">
                {columnTasks.length > 0 ? (
                    columnTasks.map(task => (
                        <TaskCard key={task.id} task={task} />
                    ))
                ) : (
                    <div className="py-8 border-2 border-dashed border-slate-300 rounded-lg flex items-center justify-center text-slate-400 text-sm italic text-center px-4">
                        Drop tasks here
                    </div>
                )}
            </div>
        </div>
    );
}, (prev, next) => {
    return prev.column.id === next.column.id;
});

export default Column;