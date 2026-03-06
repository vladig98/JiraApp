import { createFileRoute } from '@tanstack/react-router'
import TaskCard from '../../components/task';
import { useBoardStore } from '../../stores/useBoardStore';
import type Board from '../../types/board';

export const Route = createFileRoute('/boards/$boardId')({
    component: BoardView,
})

function BoardView() {
    const { boardId } = Route.useParams()
    const getBoard = useBoardStore((state) => state.getBoard);
    const board = getBoard(boardId) as Board;
    const columns = board.columns;

    return (
        <div className="flex flex-col h-screen bg-slate-50">
            {/* Header Area */}
            <header className="px-8 py-5 bg-white border-b border-slate-200 flex justify-between items-center shrink-0">
                <div>
                    <nav className="text-[10px] text-slate-400 uppercase tracking-widest font-bold mb-1">
                        Board ID: {boardId}
                    </nav>
                    <h1 className="text-xl font-bold text-slate-900">Sprint Optimization Engine</h1>
                </div>
                <div className="flex items-center gap-4">
                    <button className="text-sm font-medium text-slate-600 hover:bg-slate-100 px-3 py-2 rounded transition-colors">
                        Settings
                    </button>
                    <button className="bg-[#0052CC] hover:bg-[#0747A6] text-white px-4 py-2 rounded font-medium shadow-sm transition-all active:scale-95">
                        + New Task
                    </button>
                </div>
            </header>

            {/* The Kanban Track */}
            <main className="flex-1 overflow-x-auto overflow-y-hidden scrollbar-thin scrollbar-thumb-slate-300 scrollbar-track-transparent">
                <div className="flex gap-6 p-8 h-full min-w-max">
                    {columns.map((col) => {
                        const columnTasks = col.tasks;

                        return (
                            <div key={col.id} className="w-80 flex flex-col max-h-full">
                                {/* Column Header */}
                                <div className="flex items-center justify-between mb-4 px-1">
                                    <div className="flex items-center gap-2">
                                        <div className="w-2 h-2 rounded-full" style={{ backgroundColor: "0052CC" }} />
                                        <h3 className="font-bold text-slate-600 text-sm uppercase tracking-wide">
                                            {col.name}
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
                    })}

                    {/* Add Column Button */}
                    <button className="w-80 h-[100px] shrink-0 border-2 border-dashed border-slate-300 rounded-xl flex items-center justify-center text-slate-500 font-medium hover:border-[#0052CC] hover:text-[#0052CC] hover:bg-blue-50/50 transition-all group">
                        <span className="text-xl mr-2 group-hover:scale-125 transition-transform">+</span>
                        Add another column
                    </button>
                </div>
            </main>
        </div>
    )
}