import { createFileRoute } from '@tanstack/react-router'
import { useEffect, useState } from 'react'
import { Link } from '@tanstack/react-router'
import { BoardActionDropdown } from '../components/BoardActionDropdown'
import type Board from '../types/Board'

export const Route = createFileRoute('/')({
    component: BoardList,
})

function BoardList() {
    const [boards, setBoards] = useState<Board[] | undefined>(undefined)
    const [isFetching, setIsFetching] = useState(false)

    useEffect(() => {
        populateBoards()
    }, [])

    async function populateBoards() {
        setIsFetching(true)
        try {
            const response = await fetch('boards')
            if (response.ok) {
                const data = await response.json() as Board[]
                setBoards([...data].sort((a, b) => a.orderIndex - b.orderIndex))
            }
        } catch (error) {
            setBoards([])
        } finally {
            setIsFetching(false)
        }
    }

    return (
        <>
            <header className="px-8 py-6 border-b border-slate-200 flex justify-between items-center bg-white">
                <div>
                    <nav className="text-xs text-slate-500 mb-1">Projects / Boards</nav>
                    <h1 className="text-2xl font-semibold text-slate-900">All Boards</h1>
                </div>
                <div className="flex gap-2">
                    {isFetching && <span className="text-sm text-slate-400 animate-pulse mt-2">Syncing...</span>}
                    <Link to="/boards/create" className="bg-[#0052CC] hover:bg-[#0747A6] text-white px-4 py-2 rounded font-medium transition-colors">
                        Create Board
                    </Link>
                </div>
            </header>

            <div className="p-8 overflow-auto">
                {boards === undefined ? (
                    <div className="flex flex-col items-center justify-center h-64 border-2 border-dashed border-slate-200 rounded-lg">
                        <div className="w-10 h-10 border-4 border-slate-200 border-t-[#0052CC] rounded-full animate-spin mb-4"></div>
                        <p className="text-slate-500 font-medium">Fetching your workspace...</p>
                    </div>
                ) : boards.length === 0 ? (
                    <div className="text-center py-20 bg-slate-50 rounded-lg border border-slate-200">
                        <h3 className="text-lg font-medium text-slate-900">No boards present</h3>
                        <p className="text-slate-500">You haven't created any boards in this project yet.</p>
                    </div>
                ) : (
                    <div className="flex flex-col gap-1">
                        {/* Header Row */}
                        <div className="grid grid-cols-12 px-6 py-3 text-xs uppercase text-slate-500 font-semibold bg-slate-50 border border-slate-200 rounded-t-lg">
                            <div className="col-span-6">Name</div>
                            <div className="col-span-2">Created (UTC)</div>
                            <div className="col-span-3">Last Updated (UTC)</div>
                            <div className="col-span-1 text-right">Actions</div>
                        </div>

                        {/* Data Rows */}
                        <div className="flex flex-col border-x border-b border-slate-200 rounded-b-lg divide-y divide-slate-100 bg-white shadow-sm">
                            {boards.map((board) => (
                                <Link
                                    key={board.id}
                                    to="/boards/$boardId"
                                    params={{ boardId: board.id }}
                                    className="grid grid-cols-12 px-6 py-4 items-center hover:bg-blue-50/50 transition-colors group"
                                >
                                    <div className="col-span-6 font-medium text-[#0052CC] group-hover:underline">
                                        {board.name}
                                    </div>
                                    <div className="col-span-2 text-sm text-slate-500">
                                        {new Date(board.createdAt).toLocaleDateString()}
                                    </div>
                                    <div className="col-span-3 text-sm text-slate-500">
                                        {new Date(board.updatedAt).toLocaleDateString()}
                                    </div>
                                    <div className="col-span-1 text-right relative">
                                        <BoardActionDropdown boardId={board.id} />
                                    </div>
                                </Link>
                            ))}
                        </div>
                    </div>
                )}
            </div>
        </>
    )
}