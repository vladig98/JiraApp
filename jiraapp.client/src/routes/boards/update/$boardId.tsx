import { useState } from 'react'
import { createFileRoute, useNavigate } from '@tanstack/react-router'
import { useBoardStore } from '../../../stores/useBoardStore';
import type Board from '../../../types/board';
import ErrorToast from '../../../errorToast';

export const Route = createFileRoute('/boards/update/$boardId')({
    component: EditBoardForm,
})

function EditBoardForm() {
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [apiError, setApiError] = useState<string | null>(null);
    const navigate = useNavigate();
    const { boardId } = Route.useParams()

    const getBoard = useBoardStore((state) => state.getBoard);
    const board = getBoard(boardId) as Board;
    const upsertBoard = useBoardStore((state) => state.upsert);

    const [name, setName] = useState(board?.name);

    async function handleSubmit() {
        setIsSubmitting(true);

        // Optimistic update
        const originalBoard = getBoard(boardId);
        if (!originalBoard) {
            return;
        }

        const updatedBoard = { ...originalBoard, name: name.trim() };
        upsertBoard(updatedBoard);

        try {
            const response = await fetch("/boards/" + boardId, {
                method: 'PUT',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ name: name?.trim() })
            });

            if (!response.ok) {
                throw new Error();
            }

            navigate({ to: '/boards/$boardId', params: { boardId: boardId } });
        } catch (error) {
            // Revert the optimistic update
            setApiError("The server rejected the new board name. Changes have been reverted.");
            upsertBoard(originalBoard);
        } finally {
            setIsSubmitting(false);
        }
    }

    return (
        <>
            <ErrorToast message={apiError} onDismiss={() => setApiError(null)} />
            <div className="flex flex-col min-h-full">
                <header className="px-8 py-6 border-b border-slate-200 bg-white">
                    <nav className="text-xs text-slate-500 mb-1">Projects / Boards / Update / {boardId}</nav>
                    <h1 className="text-2xl font-semibold text-slate-900">Edit Board</h1>
                </header>

                <div className="p-8 flex-1 bg-slate-50/50">
                    <div className="max-w-full">
                        <div className="p-8 bg-white border border-slate-200 rounded-lg shadow-sm">
                            <form action={handleSubmit} className="space-y-6">
                                <div>
                                    <label className="block text-sm font-semibold text-slate-700 mb-2 uppercase tracking-wide">
                                        Board Name
                                    </label>
                                    <input
                                        name="boardName"
                                        type="text"
                                        required
                                        autoFocus
                                        value={name}
                                        onChange={e => setName(e.target.value)}
                                        placeholder="e.g. Q1 Optimization Engine"
                                        className="w-full px-4 py-3 bg-white border border-slate-300 rounded text-slate-900 text-lg focus:outline-none focus:ring-2 focus:ring-[#0052CC] focus:border-transparent transition-all shadow-sm"
                                    />
                                    <p className="mt-2 text-sm text-slate-500">
                                        Explain the purpose of this board. You can change this later in the settings.
                                    </p>
                                </div>

                                <div className="flex gap-4 justify-end pt-4 border-t border-slate-100">
                                    <button
                                        type="button"
                                        onClick={() => navigate({ to: '/' })}
                                        className="px-5 py-2.5 text-slate-600 hover:bg-slate-100 rounded font-medium transition-colors"
                                    >
                                        Cancel
                                    </button>
                                    <button
                                        type="submit"
                                        disabled={isSubmitting}
                                        className="bg-[#0052CC] hover:bg-[#0747A6] disabled:bg-slate-400 text-white px-8 py-2.5 rounded font-medium transition-all flex items-center gap-3 shadow-md active:scale-95"
                                    >
                                        {isSubmitting ? (
                                            <>
                                                <div className="w-4 h-4 border-2 border-white/30 border-t-white rounded-full animate-spin" />
                                                Updating...
                                            </>
                                        ) : (
                                            'Update Board'
                                        )}
                                    </button>
                                </div>
                            </form>
                        </div>
                    </div>
                </div>
            </div>
        </>
    );
}