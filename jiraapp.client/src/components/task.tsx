import { memo } from 'react';
import type TaskCardProps from '../types/taskCardProps';

const TaskCard = memo(({ task }: TaskCardProps) => {
    return (
        <div className="group bg-white p-4 rounded-lg border border-slate-200 shadow-sm hover:border-[#0052CC] hover:shadow-md transition-all cursor-grab active:cursor-grabbing">
            <div className="flex justify-between items-start mb-2">
                <span className="text-[10px] text-slate-400 font-mono self-center">
                    idx: {task.orderIndex}
                </span>
            </div>

            <h4 className="text-sm font-bold text-slate-900 leading-tight mb-1">
                {task.title}
            </h4>

            <p className="text-xs text-slate-600 line-clamp-2 mb-4 italic">
                {task.description || "No description provided."}
            </p>

            <div className="flex justify-between items-center border-t border-slate-50 pt-3">
                <div className="flex items-center gap-2">
                    <div className="w-5 h-5 rounded-full bg-[#0052CC] flex items-center justify-center text-[8px] font-bold text-white">
                        AZ
                    </div>
                </div>
                <div className="text-[9px] text-slate-300" title={`Version: ${task.version}`}>
                    v.{task.version.substring(0, 4)}
                </div>
            </div>
        </div>
    );
}, (prev, next) => {
    return prev.task.id === next.task.id && prev.task.version === next.task.version;
});

export default TaskCard;