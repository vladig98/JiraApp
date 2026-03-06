import { createFileRoute, useNavigate } from '@tanstack/react-router'
import { useState } from 'react'
import ErrorToast from '../../../errorToast';
import { useBoardStore } from '../../../stores/useBoardStore';

export const Route = createFileRoute('/columns/delete/$columnId')({
    component: EditColumnPage,
})

function EditColumnPage() {
    const navigate = useNavigate();
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [apiError, setApiError] = useState<string | null>(null);
    const { columnId } = Route.useParams()

    const getColumn = useBoardStore((state) => state.getColumn);
    const getBoard = useBoardStore((state) => state.getBoardByColumnId);
    const upsertColumn = useBoardStore((state) => state.upsertColumn);
    const deleteColumn = useBoardStore((state) => state.removeColumn);

    const column = getColumn(columnId);
    const board = getBoard(columnId);
    const boardId = board?.id ?? "";

    const [title, setTitle] = useState(column.name);

    async function handleSubmit(e: React.SubmitEvent) {
        e.preventDefault();
        setIsSubmitting(true);

        const originalColumn = getColumn(columnId);
        if (!originalColumn) {
            return;
        }

        deleteColumn(columnId);

        try {
            const response = await fetch("/columns/" + columnId, {
                method: 'DELETE'
            });

            if (!response.ok) {
                throw new Error();
            }

            navigate({ to: '/boards/$boardId', params: { boardId: boardId } });
        } catch (error) {
            setApiError("The server rejected the column deletion. No changes were made.");
            upsertColumn(originalColumn, boardId);
        } finally {
            setIsSubmitting(false);
        }
    }

    return (
        <>
            <ErrorToast message={apiError} onDismiss={() => setApiError(null)} />
            <div className="flex flex-col min-h-screen bg-slate-50/50">
                {/* Header */}
                <header className="px-8 py-6 border-b border-slate-200 bg-white">
                    <nav className="text-xs text-slate-500 mb-1 uppercase tracking-wider font-bold">
                        Projects / Boards / {boardId} / Columns / {columnId} / Delete
                    </nav>
                    <h1 className="text-2xl font-semibold text-slate-900">Delete Column</h1>
                </header>

                {/* Form Container */}
                <main className="p-8 flex justify-center">
                    <div className="w-full max-w-2xl bg-white border border-slate-200 rounded-xl shadow-sm overflow-hidden">
                        <form onSubmit={handleSubmit} className="p-8 space-y-8">

                            {/* Title Input */}
                            <div className="space-y-2">
                                <label className="block text-sm font-bold text-slate-700 uppercase tracking-wide">
                                    Column Title
                                </label>
                                <input
                                    required
                                    autoFocus
                                    disabled
                                    readOnly
                                    type="text"
                                    value={title}
                                    onChange={(e) => setTitle(e.target.value)}
                                    placeholder="e.g., Quality Assurance"
                                    className="w-full px-4 py-3 bg-[#EBEBE4] border border-slate-300 rounded focus:ring-2 focus:ring-[#0052CC] focus:border-transparent outline-none transition-all text-lg shadow-sm"
                                />
                                <p className="text-xs text-slate-500 italic">
                                    This will appear at the top of your Kanban track.
                                </p>
                            </div>

                            {/* Actions */}
                            <div className="flex items-center justify-end gap-4 pt-6 border-t border-slate-100">
                                <button
                                    type="button"
                                    onClick={() => navigate({ to: '/boards/$boardId', params: { boardId: boardId } })}
                                    className="px-6 py-2.5 text-slate-600 font-medium hover:bg-slate-100 rounded transition-colors"
                                >
                                    Cancel
                                </button>
                                <button
                                    type="submit"
                                    disabled={isSubmitting || !title.trim()}
                                    className="bg-[#880000] hover:bg-[#550000] disabled:bg-slate-300 text-white px-8 py-2.5 rounded font-bold shadow-md transition-all active:scale-95 flex items-center gap-2"
                                >
                                    {isSubmitting ? 'Deleting...' : 'Delete Column'}
                                </button>
                            </div>
                        </form>
                    </div>
                </main>
            </div>
        </>
    );
}