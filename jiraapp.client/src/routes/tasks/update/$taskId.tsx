import { createFileRoute, useNavigate } from '@tanstack/react-router'
import { useState } from 'react'
import { useBoardStore } from '../../../stores/useBoardStore'
import ErrorToast from '../../../errorToast'
import type Task from '../../../types/task';
import type Board from '../../../types/board';
import type Column from '../../../types/column';

export const Route = createFileRoute('/tasks/update/$taskId')({
    component: EditTaskPage,
})

function EditTaskPage() {
    const { taskId } = Route.useParams()
    const navigate = useNavigate()

    const getBoardByTaskId = useBoardStore((state) => state.getBoardByTaskId)
    const getColumnByTaskId = useBoardStore((state) => state.getColumnByTaskId)
    const getTask = useBoardStore((state) => state.getTask)
    const upsertTask = useBoardStore((state) => state.upsertTask)

    const board = getBoardByTaskId(taskId) as Board;
    const column = getColumnByTaskId(taskId) as Column;
    const task = getTask(taskId) as Task;

    const [isSubmitting, setIsSubmitting] = useState(false)
    const [apiError, setApiError] = useState<string | null>(null)
    const [title, setTitle] = useState(task.title)
    const [description, setDescription] = useState(task.description)

    async function handleSubmit(e: React.SubmitEvent) {
        e.preventDefault()
        setIsSubmitting(true)

        const originalTask = getTask(taskId) as Task;
        if (!originalTask) {
            return;
        }

        const updatedTask = { ...originalTask, title: title.trim(), description: description.trim() };
        upsertTask(updatedTask, column?.id);

        try {
            const response = await fetch("/tasks/" + taskId, {
                method: 'PUT',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ title: title, description: description, version: task.version })
            });

            if (!response.ok) {
                throw new Error();
            }

            const updatedTaskFromServer = await response.json() as Task;
            upsertTask(updatedTaskFromServer, column?.id);

            navigate({ to: '/boards/$boardId', params: { boardId: board.id } });
        } catch (error) {
            setApiError("The server rejected the new task details. No changes were made.")
            upsertTask(originalTask, column?.id);
        } finally {
            setIsSubmitting(false)
        }
    }

    return (
        <>
            <ErrorToast message={apiError} onDismiss={() => setApiError(null)} />
            <div className="flex flex-col min-h-screen bg-slate-50/50">
                <header className="px-8 py-6 border-b border-slate-200 bg-white">
                    <nav className="text-[10px] text-slate-400 uppercase tracking-widest font-bold mb-1">
                        Projects / Boards / {board?.id} / Columns / {column?.id} / Tasks / Edit / {taskId}
                    </nav>
                    <h1 className="text-2xl font-bold text-slate-900">Edit Task in {column?.name}</h1>
                </header>

                <main className="p-8 flex justify-center">
                    <div className="w-full max-w-2xl bg-white border border-slate-200 rounded-xl shadow-sm p-8">
                        <form onSubmit={handleSubmit} className="space-y-6">

                            <div className="space-y-2">
                                <label className="block text-xs font-bold text-slate-500 uppercase tracking-wider">
                                    Task Title
                                </label>
                                <input
                                    required
                                    autoFocus
                                    type="text"
                                    value={title}
                                    onChange={(e) => setTitle(e.target.value)}
                                    placeholder="Task summary..."
                                    className="w-full px-4 py-3 border border-slate-300 rounded focus:ring-2 focus:ring-[#0052CC] outline-none text-lg shadow-sm"
                                />
                            </div>

                            <div className="space-y-2">
                                <label className="block text-xs font-bold text-slate-500 uppercase tracking-wider">
                                    Description
                                </label>
                                <textarea
                                    value={description}
                                    onChange={(e) => setDescription(e.target.value)}
                                    rows={5}
                                    placeholder="Add technical details or acceptance criteria..."
                                    className="w-full px-4 py-3 border border-slate-300 rounded focus:ring-2 focus:ring-[#0052CC] outline-none resize-none shadow-sm"
                                />
                            </div>

                            <div className="flex justify-end gap-3 pt-6 border-t border-slate-100">
                                <button
                                    type="button"
                                    onClick={() => window.history.back()}
                                    className="px-6 py-2 text-slate-600 font-medium hover:bg-slate-100 rounded transition-colors"
                                >
                                    Cancel
                                </button>
                                <button
                                    type="submit"
                                    disabled={isSubmitting || !title.trim()}
                                    className="bg-[#0052CC] hover:bg-[#0747A6] disabled:bg-slate-300 text-white px-8 py-2.5 rounded font-bold shadow-md transition-all active:scale-95"
                                >
                                    {isSubmitting ? 'Optimizing...' : 'Update Task'}
                                </button>
                            </div>
                        </form>
                    </div>
                </main>
            </div>
        </>
    )
}