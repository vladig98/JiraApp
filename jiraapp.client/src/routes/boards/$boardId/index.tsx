import { createFileRoute, useNavigate } from '@tanstack/react-router'
import Column from '../../../components/column';
import { useBoardStore } from '../../../stores/useBoardStore';
import type Board from '../../../types/board';

export const Route = createFileRoute('/boards/$boardId/')({
    component: BoardIndex,
})

function BoardIndex() {
    const { boardId } = Route.useParams()
    const getBoard = useBoardStore((state) => state.getBoard);
    const board = getBoard(boardId) as Board;
    const columns = board.columns;
    const navigate = useNavigate();

    function goToCreateColumn() {
        navigate({ to: '/boards/$boardId/columns/create', params: { boardId: boardId } });
    }

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
            </header>

            {/* The Kanban Track */}
            <main className="flex-1 overflow-x-auto overflow-y-hidden scrollbar-thin scrollbar-thumb-slate-300 scrollbar-track-transparent">
                <div className="flex gap-6 p-8 h-full min-w-max">
                    {columns.map(col => (
                        <Column key={col.id} column={col} />
                    ))}

                    {/* Add Column Button */}
                    <button onClick={goToCreateColumn} className="w-80 h-[100px] shrink-0 border-2 border-dashed border-slate-300 rounded-xl flex items-center justify-center text-slate-500 font-medium hover:border-[#0052CC] hover:text-[#0052CC] hover:bg-blue-50/50 transition-all group">
                        <span className="text-xl mr-2 group-hover:scale-125 transition-transform">+</span>
                        Add another column
                    </button>
                </div>
            </main>
        </div>
    )
}