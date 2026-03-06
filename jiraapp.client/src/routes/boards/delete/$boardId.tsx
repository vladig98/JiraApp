import { useState } from 'react'
import { createFileRoute, useNavigate } from '@tanstack/react-router'
import { useBoardStore } from '../../../stores/useBoardStore';
import type Board from '../../../types/board';
import ErrorToast from '../../../errorToast';

export const Route = createFileRoute('/boards/delete/$boardId')({
    component: DeleteBoardForm,
})

function DeleteBoardForm() {
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [apiError, setApiError] = useState<string | null>(null);
    const navigate = useNavigate();
    const { boardId } = Route.useParams()

    const getBoard = useBoardStore((state) => state.getBoard);
    const board = getBoard(boardId) as Board;
    const upsertBoard = useBoardStore((state) => state.upsert);
    const removeBoard = useBoardStore((state) => state.remove);

    const [name, setName] = useState(board?.name);

    async function handleSubmit(formData: FormData) {
        setIsSubmitting(true);

        // Optimistic update
        const originalBoard = getBoard(boardId);
        if (!originalBoard) {
            return;
        }

        removeBoard(boardId);

        try {
            const response = await fetch("/boards/" + boardId, {
                method: 'DELETE'
            });

            if (!response.ok) {
                throw new Error();
            }

            navigate({ to: '/', params: boardId });
        } catch (error) {
            // Revert the optimistic update
            setApiError("The server rejected the deletion. Changes have been reverted.");
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
                    <nav className="text-xs text-slate-500 mb-1">Projects / Boards / Delete / {boardId}</nav>
                    <h1 className="text-2xl font-semibold text-slate-900">Delete Board</h1>
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
                                        disabled
                                        readOnly
                                        autoFocus
                                        value={name}
                                        placeholder="e.g. Q1 Optimization Engine"
                                        className="w-full px-4 py-3 bg-[#EBEBE4] border border-slate-300 rounded text-slate-900 text-lg focus:outline-none focus:ring-2 focus:ring-[#0052CC] focus:border-transparent transition-all shadow-sm"
                                    />
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
                                        className="bg-[#880000] hover:bg-[#550000] disabled:bg-slate-400 text-white px-8 py-2.5 rounded font-medium transition-all flex items-center gap-3 shadow-md active:scale-95"
                                    >
                                        {isSubmitting ? (
                                            <>
                                                <div className="w-4 h-4 border-2 border-white/30 border-t-white rounded-full animate-spin" />
                                                Deleting...
                                            </>
                                        ) : (
                                            'Delete Board'
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